
import { Bodies, BodyTemplate, CreepState, CreepType, Customer, CustomerPriorities, Positions, SiteId, Vector, Vectors, _Customer } from "./Common";
import { CreepBase, CreepBaseMemory, Creeps } from "./Creeps";
import { profile } from "./Profiling";
import { Site, Sites } from "./Sites";

const BUILDER_TEMPLATE: BodyTemplate = BodyTemplate.create([WORK, CARRY, MOVE, MOVE], 12);

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

export class Builder extends CreepBase<BuilderMemory> implements _Customer
{
    get site(): Site | undefined { return Sites.get(this.memory.site); }
    set site(value: Site | undefined) { this.memory.site = value?.id; }

    readonly maxEnergyPerTick: number;

    readonly customer: Customer;
    readonly priority: number;
    demand: number;

    constructor(creep: Creep)
    {
        super(creep, CreepType.Builder);

        this.maxEnergyPerTick = this.workParts * BUILD_POWER;

        this.customer = creep;
        this.priority = CustomerPriorities["Builder"];
        this.demand = this.freeEnergyCapacity;
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
    private static _all: Vector<Builder> = new Vector();

    static get all(): Vector<Builder> { return Builders._all.clone(); }
    static get maxEnergyPerTick(): number { return Builders._all.sum(b => b.maxEnergyPerTick); }

    static initialize()
    {
        Builders._all = Creeps.ofType(CreepType.Builder).map(c => new Builder(c));

        Bodies.register(CreepType.Builder, BUILDER_TEMPLATE);
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
                result.append(builder);
            }
            else
            {
                let site: Site | undefined = Builders.findSite(builder)

                if (site)
                {
                    builder.site = site;
                    result.append(builder);
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