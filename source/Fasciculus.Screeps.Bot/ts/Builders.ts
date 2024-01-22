import * as _ from "lodash";

import { Bodies, BodyTemplate } from "./Bodies";
import { CreepBase, Creeps } from "./Creeps";
import { CreepState, CreepType } from "./Enums";
import { BuilderMemory } from "./Memories";
import { Site, Sites } from "./Sites";

const BUILDER_TEMPLATE: BodyTemplate =
{
    chunks:
        [
            { cost: 250, parts: [WORK, CARRY, MOVE, MOVE] },
            { cost: 250, parts: [WORK, CARRY, MOVE, MOVE] },
            { cost: 250, parts: [WORK, CARRY, MOVE, MOVE] },
            { cost: 250, parts: [WORK, CARRY, MOVE, MOVE] },
            { cost: 250, parts: [WORK, CARRY, MOVE, MOVE] }
        ]
};

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

    constructor(creep: Creep)
    {
        super(creep);
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
    private static _all: Builder[] = [];

    static get all(): Builder[] { return Builders._all; }

    static get maxEnergyPerTick(): number { return _.sum(Builders._all.map(b => b.capabilities.work)) * 5; }

    static initialize()
    {
        Builders._all = Creeps.ofType(CreepType.Builder).map(c => new Builder(c));

        Bodies.register(CreepType.Builder, BUILDER_TEMPLATE);
    }

    static run()
    {
        Builders._all.forEach(b => b.prepare());
        Builders.assign();
        Builders._all.forEach(b => b.prepare());
        Builders._all.forEach(b => b.execute());
    }

    private static assign()
    {
        let unassignedBuilders: _.Dictionary<Builder> = _.indexBy(Builders._all.filter(b => !b.site), "name");

        if (_.size(unassignedBuilders) == 0) return;

        let sites = Sites.all.sort(Builders.compareSites);

        for (let i = 0, n = sites.length; i < n; ++i)
        {
            let site = sites[i];

            if (site.remaining > 400)
            {
                var assignees: Builder[] = _.values(unassignedBuilders);

                assignees.forEach(b => b.site = site);
                break;
            }

            let assignables: Builder[] = _.values(unassignedBuilders);
            let builder = site.pos.findClosestByPath(assignables) || undefined;

            if (!builder) continue;

            builder.site = site;
            delete unassignedBuilders[builder.name];

            if (_.size(unassignedBuilders) == 0) break;
        }
    }

    static compareSites(a: Site, b: Site): number
    {
        return a.remaining - b.remaining;
    }
}