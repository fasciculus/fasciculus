
import { Bodies, BodyTemplate } from "./Bodies";
import { CreepBase, Creeps } from "./Creeps";
import { CreepState, CreepType } from "./Enums";
import { BuilderMemory } from "./Memories";
import { Site, Sites } from "./Sites";
import { Vector, Vectors } from "./Collections";

const BUILDER_TEMPLATE: BodyTemplate = BodyTemplate.create([WORK, CARRY, MOVE, MOVE], 12);

const BUILDER_MOVE_TO_OPTS: MoveToOpts =
{
    visualizePathStyle:
    {
        stroke: "#fff"
    }
};

export class Builder extends CreepBase
{
    get memory(): BuilderMemory { return super.memory as BuilderMemory; }

    get site(): Site | undefined { return Sites.get(this.memory.site); }
    set site(value: Site | undefined) { this.memory.site = value?.id; }

    readonly maxEnergyPerTick: number;

    constructor(creep: Creep)
    {
        super(creep);

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
    private static _all: Vector<Builder> = new Vector();

    static get maxEnergyPerTick(): number { return Builders._all.sum(b => b.maxEnergyPerTick); }

    static initialize()
    {
        Builders._all = Creeps.ofType(CreepType.Builder).map(c => new Builder(c));

        Bodies.register(CreepType.Builder, BUILDER_TEMPLATE);
    }

    static run()
    {
        Builders._all.forEach(b => b.prepare());
        Builders.assign().forEach(b => b.prepare());
        Builders._all.forEach(b => b.execute());
    }

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
        let assigned: Set<Id<ConstructionSite>> = new Set(Builders.assignedSites.map(s => s.id));
        let unassigned: Site[] = Sites.all.filter(s => !assigned.has(s.id)).values;

        if (unassigned.length == 0) return result;

        let smallSites: Site[] = unassigned.filter(s => s.remaining < 10);

        if (smallSites.length > 0)
        {
            result = builder.pos.findClosestByPath(smallSites) || undefined;
        }

        if (!result)
        {
            result = unassigned.sort(Builders.compareSites)[0];
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