import { CreepState, CreepType, Stores } from "./Common";
import { CreepBase, CreepBaseMemory, Creeps } from "./Creeps";
import { Controller, Controllers, Spawns } from "./Infrastructure";
import { profile } from "./Profiling";
import { Repairs } from "./Repairs";
import { Well, Wells } from "./Resources";
import { Site, Sites } from "./Sites";
import { Paths } from "./Travelling";

type BuilderSupply = StructureSpawn | StructureContainer;
type BuilderSupplyId = Id<StructureSpawn | StructureContainer>;

interface BuilderMemory extends CreepBaseMemory
{
    supply?: BuilderSupplyId;
    site?: SiteId;
}

export class Builder extends CreepBase<BuilderMemory>
{
    get supply(): BuilderSupply | undefined { return Game.get<BuilderSupply>(this.memory.supply); }
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

    @profile
    private executeToSite()
    {
        let site = this.site;

        if (!site) return;

        this.moveTo(site, 2, false);
    }

    @profile
    private executeToSupply()
    {
        const supply = this.supply;

        if (!supply) return;

        this.moveTo(supply, 1, false);
    }

    @profile
    private executeWithdraw()
    {
        const supply = this.supply;

        if (!supply) return;

        this.withdraw(supply, RESOURCE_ENERGY);
    }

    @profile
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
    private static _builders: Map<string, Builder> = new Map();
    private static _all: Array<Builder> = new Array();
    private static _maxEnergyPerTick: number = 0;

    static get maxEnergyPerTick(): number { return Builders._maxEnergyPerTick; }

    @profile
    static initialize()
    {
        if (Creeps.update(Builders._builders, CreepType.Builder, name => new Builder(name)))
        {
            Builders._all = Builders._builders.vs();
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
    private static prepare(builders: Array<Builder>)
    {
        builders.forEach(b => b.prepare());
        builders.filter(b => b.state == CreepState.Build).forEach(b => Paths.block(b.creep));
    }

    private static execute(builders: Array<Builder>)
    {
        builders.forEach(b => b.execute());
    }

    @profile
    private static assign(): Array<Builder>
    {
        var result: Array<Builder> = new Array();
        const unassigned: Array<Builder> = Builders._all.filter(t => !t.spawning && t.state == CreepState.Idle);

        if (unassigned.length == 0) return result;

        const empty: Array<Builder> = new Array();
        const full: Array<Builder> = new Array();

        Builders.categorize(unassigned, empty, full);

        if (empty.length > 0) result = Builders.assignEmpty(empty);
        if (full.length > 0) result = result.concat(Builders.assignFull(full));

        return result;
    }

    private static categorize(builders: Array<Builder>, empty: Array<Builder>, full: Array<Builder>)
    {
        for (const builder of builders)
        {
            if (builder.energy < builder.energyCapacity)
            {
                empty.push(builder);
            }
            else
            {
                full.push(builder);
            }
        }
    }

    private static assignEmpty(builders: Array<Builder>): Array<Builder>
    {
        const result: Array<Builder> = new Array();
        const spawns: Array<BuilderSupply> = Spawns.idle.filter(s => s.energy > 0).map(s => s.spawn);

        if (spawns.length == 0) return result;

        const builder = builders[0];
        const supply = spawns[0];

        builder.supply = supply;
        result.push(builder);

        return result;
    }

    private static assignFull(builders: Array<Builder>): Array<Builder>
    {
        const result: Array<Builder> = new Array();
        const builder = builders[0];
        var site: Site | undefined = Builders.focus || Builders.findSite(builder);

        if (!site) return result;

        builder.site = site;
        result.push(builder);

        return result;
    }

    private static get focus(): Site | undefined
    {
        return Builders.assignedSites.filter(s => s.remaining > 1)[0];
    }

    private static findSite(builder: Builder): Site | undefined
    {
        let result: Site | undefined = undefined;
        let assigned: Set<SiteId> = Builders.assignedSites.map(s => s.id).toSet();
        let unassigned: Array<Site> = Sites.all.filter(s => !assigned.has(s.id));

        if (unassigned.length == 0) return result;

        let smallSites: Array<Site> = unassigned.filter(s => s.remaining < 10);

        if (smallSites.length > 0)
        {
            result = Paths.closest(builder, smallSites, 1);
        }

        if (!result)
        {
            result = unassigned.sort(Builders.compareSites)[0];
        }

        return result;
    }

    private static get assignedSites(): Array<Site>
    {
        return Array.defined(Builders._all.map(b => b.site));
    }

    private static compareSites(a: Site, b: Site): number
    {
        return a.remaining - b.remaining;
    }
}

class DamageHelper
{
    static damageOf(repairable: Repairable): number
    {
        const avg: number = StructureWall.my.avg(w => w.hits);
        let hitsMax = repairable instanceof StructureWall ? avg + 2000 : repairable.hitsMax;

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

        this.moveTo(repairable, 2);
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
    private static _repairers: Map<string, Repairer> = new Map();
    private static _all: Array<Repairer> = new Array();
    private static _maxEnergyPerTick: number = 0;

    static get all(): Array<Repairer> { return Repairers._all.clone(); }
    static get maxEnergyPerTick(): number { return Repairers._maxEnergyPerTick; }

    @profile
    static initialize()
    {
        if (Creeps.update(Repairers._repairers, CreepType.Repairer, name => new Repairer(name)))
        {
            Repairers._all = Repairers._repairers.vs();
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

    private static assign(): Array<Repairer>
    {
        const result: Array<Repairer> = new Array();
        const unassigned: Map<string, Repairer> = Repairers.unassignedRepairers;
        const repairables: Array<Repairable> = Repairs.all.map(Repairers.toDamageInfo).sort(Repairers.compareDamage).map(d => d.repairable);

        for (let repairable of repairables)
        {
            let assignables: Array<Repairer> = unassigned.vs();
            let repairer: Repairer | undefined = Paths.closest(repairable, assignables, 2);

            if (!repairer) continue;

            repairer.repairable = repairable;
            result.push(repairer);
            unassigned.delete(repairer.name);
        }

        return result;
    }

    private static get unassignedRepairers(): Map<string, Repairer>
    {
        return Repairers._all.filter(r => !r.repairable).indexBy(r => r.name);
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

        this.moveTo(controller, 2, false);
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
    private static _upgraders: Map<string, Upgrader> = new Map();
    private static _all: Array<Upgrader> = new Array();
    private static _maxEnergyPerTick: number = 0;

    static get all(): Array<Upgrader> { return Upgraders._all.clone(); }
    static get count(): number { return Upgraders._all.length; }
    static get maxEnergyPerTick(): number { return Upgraders._maxEnergyPerTick; }

    @profile
    static initialize()
    {
        if (Creeps.update(Upgraders._upgraders, CreepType.Upgrader, name => new Upgrader(name)))
        {
            Upgraders._all = Upgraders._upgraders.vs();
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
    private static prepare(upgraders: Array<Upgrader>)
    {
        upgraders.forEach(u => u.prepare());
    }

    private static execute(upgraders: Array<Upgrader>)
    {
        upgraders.forEach(u => u.execute());
    }

    private static assign(): Array<Upgrader>
    {
        const result: Array<Upgrader> = new Array();
        const unassigned: Array<Upgrader> = Upgraders._all.filter(u => !u.controller);

        if (unassigned.length == 0) return result;

        const works: Array<ControllerWork> = Upgraders.getControllerWorks();

        if (works.length == 0) return result;

        const sorted: Array<ControllerWork> = works.sort((a, b) => a.work - b.work);
        const controller: Controller | undefined = sorted[0]?.controller;

        if (!controller) return result;

        const upgrader: Upgrader | undefined = unassigned[0];

        if (!upgrader) return result;

        upgrader.controller = controller;
        result.push(upgrader);

        return result;
    }

    private static getControllerWorks(): Array<ControllerWork>
    {
        const result: Array<ControllerWork> = Controllers.my.map(Upgraders.createControllerWork);
        const byId: Map<ControllerId, ControllerWork> = result.indexBy(cw => cw.controller.id);

        for (const upgrader of Upgraders._all)
        {
            const controller = upgrader.controller;

            if (!controller) continue;

            var entry = byId.get(controller.id);

            if (entry)
            {
                entry.work += upgrader.workParts;
            }
        }

        return result;
    }

    private static createControllerWork(controller: Controller): ControllerWork
    {
        var result: ControllerWork = { controller, work: 0 };

        return result;
    }
}

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

        this.moveTo(well, 1, false);
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
    private static _wellers: Map<string, Weller> = new Map();
    private static _all: Array<Weller> = new Array();

    private static _maxEnergyPerTick: number = 0;

    static get(name?: string): Weller | undefined { return name ? Wellers._wellers.get(name) : undefined; }

    static get count(): number { return Wellers._wellers.size; }
    static get all(): Array<Weller> { return Array.from(Wellers._all); }

    static get maxEnergyPerTick(): number { return Wellers._maxEnergyPerTick; }

    @profile
    static initialize()
    {
        if (Creeps.update(Wellers._wellers, CreepType.Weller, name => new Weller(name)))
        {
            Wellers._all = Wellers._wellers.vs();
            Wellers._maxEnergyPerTick = Wellers._all.sum(w => w.maxEnergyPerTick);
        }
    }

    static run()
    {
        Wellers.prepare(Wellers._all);
        Wellers.prepare(Wellers.assign());
        Wellers.execute(Wellers._all);
    }

    @profile
    private static prepare(wellers: Array<Weller>)
    {
        wellers.forEach(w => w.prepare());
        wellers.filter(w => w.state == CreepState.Harvest).forEach(w => Paths.block(w.creep));
    }

    private static execute(wellers: Array<Weller>)
    {
        wellers.forEach(w => w.execute());
    }

    @profile
    private static assign(): Array<Weller>
    {
        var result: Array<Weller> = new Array();
        let unassignedWellers: Array<Weller> = Wellers._all.filter(w => !w.well);

        for (let weller of unassignedWellers)
        {
            let assignableWells: Array<Well> = Wells.assignable;
            let nearestWell: Well | undefined = Paths.closest(weller, assignableWells, 1);

            if (!nearestWell) continue;

            weller.well = nearestWell;
            nearestWell.assignee = weller.creep;
            result.push(weller);
            break;
        }

        return result;
    }
}
