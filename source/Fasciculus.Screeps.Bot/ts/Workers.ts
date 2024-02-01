import { CreepState, CreepType, Dictionaries, Dictionary, Positions, Repairable, RepairableId, SiteId, Vector, Vectors } from "./Common";
import { CreepBase, CreepBaseMemory, Creeps } from "./Creeps";
import { Walls } from "./Infrastructure";
import { profile } from "./Profiling";
import { Repairs } from "./Repairs";
import { Site, Sites } from "./Sites";

const BUILDER_MOVE_TO_OPTS: MoveToOpts =
{
    visualizePathStyle:
    {
        stroke: "#fff"
    }
};

interface BuilderMemory extends CreepBaseMemory
{
    site?: SiteId;
}

export class Builder extends CreepBase<BuilderMemory>
{
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
            case CreepState.ToSite: this.executeToSite(); break;
            case CreepState.Build: this.executeBuild(); break;
        }
    }

    private executeToSite()
    {
        let site = this.site;

        if (!site) return;

        this.moveTo(site, BUILDER_MOVE_TO_OPTS);
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
            case CreepState.ToSite: this.state = this.prepareToSite(); break;
            case CreepState.Build: this.state = this.prepareBuild(); break;
        }
    }

    private prepareIdle(): CreepState
    {
        let site = this.site;

        if (!site) return CreepState.Idle;

        return this.inRangeTo(site) ? CreepState.Build : CreepState.ToSite;
    }

    private prepareToSite(): CreepState
    {
        let site = this.site;

        if (!site) return this.prepareIdle();

        return this.inRangeTo(site) ? CreepState.Build : CreepState.ToSite;
    }

    private prepareBuild()
    {
        let site = this.site;

        if (!site) return this.prepareIdle();

        return this.inRangeTo(site) ? CreepState.Build : CreepState.ToSite;
    }

    private inRangeTo(site: Site): boolean
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
        let unassigned: Vector<Builder> = Builders._all.filter(b => !b.site);

        for (let builder of unassigned)
        {
            let focus: Site | undefined = Builders.focus;

            if (focus)
            {
                builder.site = focus;
                result.add(builder);
            }
            else
            {
                let site: Site | undefined = Builders.findSite(builder)

                if (site)
                {
                    builder.site = site;
                    result.add(builder);
                }
            }
        }

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
        let smallSites: Vector<Site> = unassigned.filter(s => s.remaining < 10);

        if (smallSites.length > 0)
        {
            result = Positions.closestByPath(builder, smallSites);
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
            let repairer: Repairer | undefined = Positions.closestByPath(repairable, assignables);

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