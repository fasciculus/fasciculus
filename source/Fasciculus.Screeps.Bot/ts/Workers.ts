import { ControllerId, CreepState, CreepType, Dictionaries, Dictionary, GameWrap, Positions, Repairable, RepairableId, SiteId, SourceId, Stores, Vector, Vectors } from "./Common";
import { CreepBase, CreepBaseMemory, Creeps } from "./Creeps";
import { Controller, Controllers, Spawns, Walls } from "./Infrastructure";
import { profile } from "./Profiling";
import { Repairs } from "./Repairs";
import { Well, Wells } from "./Resources";
import { Site, Sites } from "./Sites";

const BUILDER_MOVE_TO_OPTS: MoveToOpts =
{
    visualizePathStyle:
    {
        stroke: "#fff"
    }
};

type BuilderSupply = StructureSpawn | StructureContainer;
type BuilderSupplyId = Id<StructureSpawn | StructureContainer>;

interface BuilderMemory extends CreepBaseMemory
{
    supply?: BuilderSupplyId;
    site?: SiteId;
}

export class Builder extends CreepBase<BuilderMemory>
{
    get supply(): BuilderSupply | undefined { return GameWrap.get<BuilderSupply>(this.memory.supply); }
    set supply(value: BuilderSupply | undefined) { this.memory.supply = value?.id; }

    get site(): Site | undefined { return Sites.get(this.memory.site); }
    set site(value: Site | undefined) { this.memory.site = value?.id; }

    readonly maxEnergyPerTick: number;

    constructor(name: string)
    {
        super(name);

        this.maxEnergyPerTick = this.workParts * BUILD_POWER;
    }

    execute()
    {
        switch (this.state)
        {
            case CreepState.ToSupply: this.executeToSupply(); break;
            case CreepState.ToSite: this.executeToSite(); break;
            case CreepState.Withdraw: this.executeWithdraw(); break;
            case CreepState.Build: this.executeBuild(); break;
        }
    }

    private executeToSite()
    {
        let site = this.site;

        if (!site) return;

        this.moveTo(site, BUILDER_MOVE_TO_OPTS);
    }

    @profile
    private executeToSupply()
    {
        const supply = this.supply;

        if (!supply) return;

        this.moveTo(supply);
    }

    @profile
    private executeWithdraw()
    {
        const supply = this.supply;

        if (!supply) return;

        this.withdraw(supply, RESOURCE_ENERGY);
    }

    private executeBuild()
    {
        let site = this.site;

        if (!site) return;

        this.build(site.site);
    }

    prepare()
    {
        switch (this.state)
        {
            case CreepState.Idle: this.state = this.prepareIdle(); break;
            case CreepState.ToSupply: this.state = this.prepareToSupply(); break;
            case CreepState.Withdraw: this.state = this.prepareWithdraw(); break;
            case CreepState.ToSite: this.state = this.prepareToSite(); break;
            case CreepState.Build: this.state = this.prepareBuild(); break;
        }
    }

    private prepareIdle(): CreepState
    {
        if (this.energy == 0)
        {
            const supply = this.supply;

            if (!supply) return CreepState.Idle;

            return this.inRangeToSupply(supply) ? CreepState.Withdraw : CreepState.ToSupply;
        }
        else
        {
            const site = this.site;

            if (!site) return CreepState.Idle;

            return this.inRangeToSite(site) ? CreepState.Build : CreepState.ToSite;
        }
    }

    private prepareToSupply(): CreepState
    {
        const supply = this.supply;

        if (!supply) return CreepState.Idle;

        if (Stores.energy(supply) == 0)
        {
            this.supply = undefined;
            return CreepState.Idle;
        }

        return this.inRangeToSupply(supply) ? CreepState.Withdraw : CreepState.ToSupply;
    }

    private prepareWithdraw(): CreepState
    {
        if (this.freeEnergyCapacity == 0)
        {
            this.supply = undefined;
            return CreepState.Idle;
        }

        const supply = this.supply;

        if (!supply) return CreepState.Idle;

        if (Stores.energy(supply) == 0)
        {
            this.supply = undefined;
            return CreepState.Idle;
        }

        return CreepState.Withdraw;
    }

    private prepareToSite(): CreepState
    {
        let site = this.site;

        if (!site) return CreepState.Idle;

        return this.inRangeToSite(site) ? CreepState.Build : CreepState.ToSite;
    }

    private prepareBuild()
    {
        return this.energy > 0 && this.site ? CreepState.Build : CreepState.Idle;
    }

    private inRangeToSupply(supply: BuilderSupply)
    {
        return this.pos.inRangeTo(supply, 1);
    }

    private inRangeToSite(site: Site): boolean
    {
        return this.pos.inRangeTo(site, 2);
    }
}

export class Builders
{
    private static _builders: Dictionary<Builder> = {};
    private static _all: Vector<Builder> = new Vector();
    private static _maxEnergyPerTick: number = 0;

    static get maxEnergyPerTick(): number { return Builders._maxEnergyPerTick; }

    @profile
    static initialize(reset: boolean)
    {
        if (reset)
        {
            Builders._builders = {};
            Builders._all = new Vector();
            Builders._maxEnergyPerTick = 0;
        }

        if (Creeps.update(Builders._builders, CreepType.Builder, name => new Builder(name)))
        {
            Builders._all = Dictionaries.values(Builders._builders);
            Builders._maxEnergyPerTick = Builders._all.sum(b => b.maxEnergyPerTick);
        }
    }

    static run()
    {
        Builders.prepare(Builders._all);
        Builders.prepare(Builders.assign());
        Builders.execute(Builders._all);
    }

    @profile
    private static prepare(builders: Vector<Builder>)
    {
        builders.forEach(b => b.prepare());
    }

    @profile
    private static execute(builders: Vector<Builder>)
    {
        builders.forEach(b => b.execute());
    }

    @profile
    private static assign(): Vector<Builder>
    {
        var result: Vector<Builder> = new Vector();
        const unassigned: Vector<Builder> = Builders._all.filter(t => !t.spawning && t.state == CreepState.Idle);

        if (unassigned.length == 0) return result;

        const empty: Vector<Builder> = new Vector();
        const full: Vector<Builder> = new Vector();

        Builders.categorize(unassigned, empty, full);

        if (empty.length > 0) result = Builders.assignEmpty(empty);
        if (full.length > 0) result = result.concat(Builders.assignFull(full));

        //let unassigned: Vector<Builder> = Builders._all.filter(b => !b.site);

        //for (let builder of unassigned)
        //{
        //    let focus: Site | undefined = Builders.focus;

        //    if (focus)
        //    {
        //        builder.site = focus;
        //        result.add(builder);
        //    }
        //    else
        //    {
        //        let site: Site | undefined = Builders.findSite(builder)

        //        if (site)
        //        {
        //            builder.site = site;
        //            result.add(builder);
        //        }
        //    }
        //}

        return result;
    }

    private static categorize(builders: Vector<Builder>, empty: Vector<Builder>, full: Vector<Builder>)
    {
        for (const builder of builders)
        {
            if (builder.energy < builder.energyCapacity)
            {
                empty.add(builder);
            }
            else
            {
                full.add(builder);
            }
        }
    }

    private static assignEmpty(builders: Vector<Builder>): Vector<Builder>
    {
        const result: Vector<Builder> = new Vector();
        const spawns: Vector<BuilderSupply> = Spawns.idle.filter(s => s.energy > 0).map(s => s.spawn);

        if (spawns.length == 0) return result;

        const builder = builders.at(0)!;
        const supply = spawns.at(0)!;

        builder.supply = supply;
        result.add(builder);

        return result;
    }

    private static assignFull(builders: Vector<Builder>): Vector<Builder>
    {
        const result: Vector<Builder> = new Vector();
        const builder = builders.at(0)!;
        var site: Site | undefined = Builders.focus || Builders.findSite(builder);

        if (!site) return result;

        builder.site = site;
        result.add(builder);

        return result;
    }

    private static get focus(): Site | undefined
    {
        return Builders.assignedSites.filter(s => s.remaining > 1).at(0);
    }

    private static findSite(builder: Builder): Site | undefined
    {
        let result: Site | undefined = undefined;
        let assigned: Set<SiteId> = Builders.assignedSites.map(s => s.id).toSet();
        let unassigned: Vector<Site> = Sites.all.filter(s => !assigned.has(s.id));

        if (unassigned.length == 0) return result;

        let smallSites: Vector<Site> = unassigned.filter(s => s.remaining < 10);

        if (smallSites.length > 0)
        {
            result = smallSites.at(0); // Positions.closestByPath(builder, smallSites);
        }

        if (!result)
        {
            result = unassigned.sort(Builders.compareSites).at(0);
        }

        return result;
    }

    private static get assignedSites(): Vector<Site>
    {
        return Vectors.defined(Builders._all.map(b => b.site));
    }

    private static compareSites(a: Site, b: Site): number
    {
        return a.remaining - b.remaining;
    }
}

const REPAIRER_MOVE_TO_OPTS: MoveToOpts =
{
    visualizePathStyle:
    {
        stroke: "#0f0"
    }
};

class DamageHelper
{
    static damageOf(repairable: Repairable): number
    {
        let hitsMax = repairable instanceof StructureWall ? Walls.avg + 2000 : repairable.hitsMax;

        return Math.max(0, hitsMax - repairable.hits);
    }
}

interface RepairerMemory extends CreepBaseMemory
{
    repairable?: RepairableId;
}

export class Repairer extends CreepBase<RepairerMemory>
{
    get repairable(): Repairable | undefined { return Repairs.get(this.memory.repairable); }
    set repairable(value: Repairable | undefined) { this.memory.repairable = value?.id; }

    readonly maxEnergyPerTick: number;

    constructor(name: string)
    {
        super(name);

        this.maxEnergyPerTick = this.workParts;
    }

    execute()
    {
        switch (this.state)
        {
            case CreepState.ToRepairable: this.executeToRepair(); break;
            case CreepState.Repair: this.executeRepair(); break;
        }
    }

    private executeToRepair()
    {
        let repairable = this.repairable;

        if (!repairable) return;

        this.moveTo(repairable, REPAIRER_MOVE_TO_OPTS);
    }

    private executeRepair()
    {
        let repairable = this.repairable;

        if (!repairable) return;

        this.repair(repairable);
    }

    prepare()
    {
        switch (this.state)
        {
            case CreepState.Idle: this.state = this.prepareIdle(); break;
            case CreepState.ToRepairable: this.state = this.prepareToRepairable(); break;
            case CreepState.Repair: this.state = this.prepareRepair(); break;
        }
    }

    private prepareIdle(): CreepState
    {
        let repairable = this.repairable;

        if (!repairable) return CreepState.Idle;

        return this.inRangeTo(repairable) ? CreepState.Repair : CreepState.ToRepairable;
    }

    private prepareToRepairable(): CreepState
    {
        let repairable = this.repairable;

        if (!repairable) return CreepState.Idle;

        return this.inRangeTo(repairable) ? CreepState.Repair : CreepState.ToRepairable;
    }

    private prepareRepair(): CreepState
    {
        let repairable = this.repairable;

        if (!repairable) return CreepState.Idle;

        if (DamageHelper.damageOf(repairable) == 0)
        {
            this.repairable = undefined;
            return CreepState.Idle;
        }

        return CreepState.Repair;
    }

    private inRangeTo(repairable: Repairable)
    {
        return this.pos.inRangeTo(repairable, 2);
    }
}

interface DamageInfo
{
    repairable: Repairable;
    damage: number;
}

export class Repairers
{
    private static _repairers: Dictionary<Repairer> = {};
    private static _all: Vector<Repairer> = new Vector();
    private static _maxEnergyPerTick: number = 0;

    static get all(): Vector<Repairer> { return Repairers._all.clone(); }
    static get maxEnergyPerTick(): number { return Repairers._maxEnergyPerTick; }

    @profile
    static initialize(reset: boolean)
    {
        if (reset)
        {
            Repairers._repairers = {};
            Repairers._all = new Vector();
            Repairers._maxEnergyPerTick = 0;
        }

        if (Creeps.update(Repairers._repairers, CreepType.Repairer, name => new Repairer(name)))
        {
            Repairers._all = Dictionaries.values(Repairers._repairers);
            Repairers._maxEnergyPerTick = Repairers._all.sum(r => r.maxEnergyPerTick);
        }
    }

    @profile
    static run()
    {
        Repairers._all.forEach(r => r.prepare());
        Repairers.assign().forEach(r => r.prepare());
        Repairers._all.forEach(r => r.execute());
    }

    private static assign(): Vector<Repairer>
    {
        var result: Vector<Repairer> = new Vector();
        var unassigned: Dictionary<Repairer> = Repairers.unassignedRepairers;

        if (Dictionaries.isEmpty(unassigned)) return result;

        for (let repairable of Repairers.repairables)
        {
            let assignables: Vector<Repairer> = Dictionaries.values(unassigned);
            let repairer: Repairer | undefined = assignables.at(0); // Positions.closestByPath(repairable, assignables);

            if (!repairer) continue;

            repairer.repairable = repairable;
            result.add(repairer);
            delete unassigned[repairer.name];

            if (Dictionaries.isEmpty(unassigned)) break;
        }

        return result;
    }

    private static get unassignedRepairers(): Dictionary<Repairer>
    {
        return Repairers._all.filter(r => !r.repairable).indexBy(r => r.name);
    }

    private static get repairables(): Vector<Repairable>
    {
        return Repairs.all.map(Repairers.toDamageInfo).sort(Repairers.compareDamage).map(d => d.repairable);
    }

    private static toDamageInfo(repairable: Repairable): DamageInfo
    {
        let result: DamageInfo = { repairable, damage: DamageHelper.damageOf(repairable) };

        return result;
    }

    private static compareDamage(a: DamageInfo, b: DamageInfo): number
    {
        return b.damage - a.damage;
    }
}

interface UpgraderMemory extends CreepBaseMemory
{
    controller?: ControllerId;
}

export class Upgrader extends CreepBase<UpgraderMemory>
{
    readonly maxEnergyPerTick: number;

    get controller(): Controller | undefined { return Controllers.get(this.memory.controller); }
    set controller(value: Controller | undefined) { this.memory.controller = value?.id; }

    constructor(name: string)
    {
        super(name);

        this.maxEnergyPerTick = this.workParts;
    }

    execute()
    {
        switch (this.state)
        {
            case CreepState.ToController: this.executeToController(); break;
            case CreepState.Upgrade: this.executeUpgrade(); break;
        }
    }

    @profile
    private executeToController()
    {
        let controller = this.controller;

        if (!controller) return;

        this.moveTo(controller);
    }

    private executeUpgrade()
    {
        let controller = this.controller;

        if (!controller) return;

        this.upgradeController(controller.controller);
    }

    prepare()
    {
        switch (this.state)
        {
            case CreepState.Idle: this.state = this.prepareIdle(); break;
            case CreepState.ToController: this.state = this.prepareToController(); break;
            case CreepState.Upgrade: this.state = this.prepareUpgrade(); break;
        }
    }

    private prepareIdle(): CreepState
    {
        let controller = this.controller;

        if (!controller) return CreepState.Idle;

        return this.inRangeTo(controller) ? CreepState.Upgrade : CreepState.ToController;
    }

    private prepareToController(): CreepState
    {
        let controller = this.controller;

        if (!controller) return CreepState.Idle;

        return this.inRangeTo(controller) ? CreepState.Upgrade : CreepState.ToController;
    }

    private prepareUpgrade(): CreepState
    {
        let controller = this.controller;

        if (!controller) return CreepState.Idle;

        return this.inRangeTo(controller) ? CreepState.Upgrade : CreepState.ToController;
    }

    private inRangeTo(controller: Controller): boolean
    {
        return this.pos.inRangeTo(controller, 2)
    }
}

interface ControllerWork
{
    controller: Controller;
    work: number;
}

export class Upgraders
{
    private static _upgraders: Dictionary<Upgrader> = {};
    private static _all: Vector<Upgrader> = new Vector();
    private static _maxEnergyPerTick: number = 0;

    static get all(): Vector<Upgrader> { return Upgraders._all.clone(); }
    static get count(): number { return Upgraders._all.length; }
    static get maxEnergyPerTick(): number { return Upgraders._maxEnergyPerTick; }

    @profile
    static initialize(reset: boolean)
    {
        if (reset)
        {
            Upgraders._upgraders = {};
            Upgraders._all = new Vector();
            Upgraders._maxEnergyPerTick = 0;
        }

        if (Creeps.update(Upgraders._upgraders, CreepType.Upgrader, name => new Upgrader(name)))
        {
            Upgraders._all = Dictionaries.values(Upgraders._upgraders);
            Upgraders._maxEnergyPerTick = Upgraders._all.sum(u => u.maxEnergyPerTick);
        }
    }

    static run()
    {
        Upgraders.prepare(Upgraders._all);
        Upgraders.prepare(Upgraders.assign());
        Upgraders.execute(Upgraders._all);
    }

    @profile
    private static prepare(upgraders: Vector<Upgrader>)
    {
        upgraders.forEach(u => u.prepare());
    }

    // @profile
    private static execute(upgraders: Vector<Upgrader>)
    {
        upgraders.forEach(u => u.execute());
    }

    private static assign(): Vector<Upgrader>
    {
        var result: Vector<Upgrader> = new Vector();
        let unassigned: Vector<Upgrader> = Upgraders._all.filter(u => !u.controller);

        if (unassigned.length == 0) return result;

        let works: Vector<ControllerWork> = Upgraders.getControllerWorks();

        if (works.length == 0) return result;

        let sorted: Vector<ControllerWork> = works.sort((a, b) => a.work - b.work);
        let controller: Controller | undefined = sorted.at(0)?.controller;

        if (!controller) return result;

        let upgrader: Upgrader | undefined = unassigned.at(0); // Positions.closestByPath(controller, unassigned);

        if (!upgrader) return result;

        upgrader.controller = controller;
        result.add(upgrader);

        return result;
    }

    private static getControllerWorks(): Vector<ControllerWork>
    {
        let result: Vector<ControllerWork> = Controllers.my.map(Upgraders.createControllerWork);
        let byId: Dictionary<ControllerWork> = result.indexBy(cw => cw.controller.id);

        for (let upgrader of Upgraders._all)
        {
            let controller = upgrader.controller;

            if (!controller) continue;

            byId[controller.id].work += upgrader.workParts;
        }

        return result;
    }

    private static createControllerWork(controller: Controller): ControllerWork
    {
        var result: ControllerWork = { controller, work: 0 };

        return result;
    }
}

const WELLER_MOVE_TO_OPTS: MoveToOpts =
{
    reusePath: 0,

    visualizePathStyle:
    {
        stroke: "#fff"
    }
};

interface WellerMemory extends CreepBaseMemory
{
    well?: SourceId;
    ready?: boolean;
}

export class Weller extends CreepBase<WellerMemory>
{
    readonly maxEnergyPerTick: number;

    get well(): Well | undefined { return Wells.get(this.memory.well); }
    set well(value: Well | undefined) { this.memory.well = value?.id; this.ready = false; }

    get ready(): boolean { return this.memory.ready || false; }
    private set ready(value: boolean) { this.memory.ready = value; }

    private get full(): boolean { return this.freeEnergyCapacity < this.maxEnergyPerTick; }

    constructor(name: string)
    {
        super(name);

        this.maxEnergyPerTick = this.workParts * HARVEST_POWER;
    }

    execute()
    {
        switch (this.state)
        {
            case CreepState.ToWell: this.executeToWell(); break;
            case CreepState.Harvest: this.executeHarvest(); break;
        }
    }

    @profile
    private executeToWell()
    {
        let well = this.well;

        if (!well) return;

        this.moveTo(well, WELLER_MOVE_TO_OPTS);
    }

    @profile
    private executeHarvest()
    {
        if (this.full) return;

        let well = this.well;

        if (!well) return;

        this.harvest(well.source);
    }

    prepare()
    {
        switch (this.state)
        {
            case CreepState.Idle: this.state = this.prepareIdle(); break;
            case CreepState.ToWell: this.state = this.prepareToWell(); break;
            case CreepState.Harvest: this.state = this.prepareHarvest(); break;
        }
    }

    @profile
    private prepareIdle(): CreepState
    {
        return this.well ? CreepState.ToWell : CreepState.Idle;
    }

    @profile
    private prepareToWell(): CreepState
    {
        let well = this.well;

        if (!well) return CreepState.Idle;

        if (this.inRangeTo(well))
        {
            this.ready = true;
            return CreepState.Harvest;
        }

        return CreepState.ToWell;
    }

    @profile
    private prepareHarvest(): CreepState
    {
        return this.well ? CreepState.Harvest : CreepState.Idle;
    }

    private inRangeTo(target: Well): boolean
    {
        return this.pos.inRangeTo(target.pos, 1);
    }
}

const FIND_CLOSEST_WELL_OPTS: FindPathOpts =
{
    ignoreCreeps: true
};

export class Wellers
{
    private static _wellers: Dictionary<Weller> = {};

    private static _all: Vector<Weller> = new Vector();
    private static _maxEnergyPerTick: number = 0;
    private static _maxEnergyCapacity: number = 0;

    static get(name?: string): Weller | undefined { return name ? Wellers._wellers[name] : undefined; }

    static get count(): number { return Wellers._all.length; }
    static get all(): Vector<Weller> { return Wellers._all.clone(); }

    static get maxEnergyPerTick(): number { return Wellers._maxEnergyPerTick; }
    static get maxEnergyCapacity(): number { return Wellers._maxEnergyCapacity; }

    @profile
    static initialize(reset: boolean)
    {
        if (reset)
        {
            Wellers._wellers = {};
            Wellers._all = new Vector();
            Wellers._maxEnergyPerTick = 0;
            Wellers._maxEnergyCapacity = 0;
        }

        if (Creeps.update(Wellers._wellers, CreepType.Weller, name => new Weller(name)))
        {
            Wellers._all = Dictionaries.values(Wellers._wellers);
            Wellers._maxEnergyPerTick = Wellers._all.sum(w => w.maxEnergyPerTick);
            Wellers._maxEnergyCapacity = Wellers._all.max(w => w.energyCapacity)?.energyCapacity || 0;
        }
    }

    static run()
    {
        Wellers.prepare(Wellers._all);
        Wellers.prepare(Wellers.assign());
        Wellers.execute(Wellers._all);
    }

    @profile
    private static prepare(wellers: Vector<Weller>)
    {
        wellers.forEach(w => w.prepare());
    }

    private static execute(wellers: Vector<Weller>)
    {
        wellers.forEach(w => w.execute());
    }

    @profile
    private static assign(): Vector<Weller>
    {
        var result: Vector<Weller> = new Vector();
        let unassignedWellers: Vector<Weller> = Wellers.all.filter(w => !w.well);

        for (let weller of unassignedWellers)
        {
            let assignableWells: Vector<Well> = Wells.assignable;
            let nearestWell: Well | undefined = assignableWells.at(0); // Positions.closestByPath(weller, assignableWells, FIND_CLOSEST_WELL_OPTS);

            if (!nearestWell) continue;

            weller.well = nearestWell;
            nearestWell.assignee = weller.creep;
            result.add(weller);
            break;
        }

        return result;
    }
}