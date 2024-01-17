import * as _ from "lodash";

import { Bodies } from "./Bodies";
import { Constructions } from "./Constructions";
import { CreepBase, CreepState, CreepType, Creeps } from "./Creeps";

const BUILDER_MOVE_TO_OPTS: MoveToOpts =
{
    visualizePathStyle:
    {
        stroke: "#fff"
    }
};

export class Builder extends CreepBase
{
    constructor(creep: Creep)
    {
        super(creep);
    }

    run()
    {
        var state = this.prepare();

        switch (state)
        {
            case CreepState.MoveToSite: this.moveTo(this.site!, BUILDER_MOVE_TO_OPTS); break;
            case CreepState.Build: this.build(this.site!); break;
        }

        this.state = state;
    }

    private prepare(): CreepState
    {
        var state = this.state;
        var site = this.site;

        switch (state)
        {
            case CreepState.Idle: return this.prepareIdle(site);
            case CreepState.MoveToSite: return this.prepareMoveToSite(site);
            case CreepState.Build: return this.prepareBuild(site);
            default: return state;
        }
    }

    private prepareIdle(site: ConstructionSite | null): CreepState
    {
        if (!site)
        {
            site = this.findSite();

            if (!site) return CreepState.Idle;

            this.site = site;
        }

        return this.pos.inRangeTo(site, 3) ? CreepState.Build : CreepState.MoveToSite;
    }

    private prepareMoveToSite(site: ConstructionSite | null): CreepState
    {
        if (!site) return this.prepareIdle(site);

        return this.pos.inRangeTo(site, 3) ? CreepState.Build : CreepState.MoveToSite;
    }

    private prepareBuild(site: ConstructionSite | null)
    {
        return site ? CreepState.Build : this.prepareIdle(site);
    }

    private findSite(): ConstructionSite | null
    {
        var served = Builders.served;
        var sites = Constructions.notWalls.filter(s => !served.has(s.id));

        if (sites.length == 0)
        {
            sites = Constructions.walls.filter(s => !served.has(s.id));
        }

        if (sites.length == 0) return null;

        let bestSite = sites[0];
        let bestWork = bestSite.progressTotal - bestSite.progress;
        let bestDist = this.pos.getRangeTo(bestSite);

        for (let i = 1, n = sites.length; i < n; ++i)
        {
            let site = sites[i];
            let work = site.progressTotal - site.progress;

            if (work < bestWork)
            {
                bestSite = site;
                bestWork = work;
                bestDist = this.pos.getRangeTo(site);
            }
            else if (work == bestWork)
            {
                let dist = this.pos.getRangeTo(site);

                if (dist < bestDist)
                {
                    bestSite = site;
                    bestWork = work;
                    bestDist = dist;
                }
            }
        }

        return bestSite;
    }
}

const BUILDER_PARTS: BodyPartConstant[] = [CARRY, WORK, MOVE, CARRY, WORK, MOVE, CARRY, WORK, MOVE, CARRY, WORK, MOVE];
const BUILDER_MIN_SIZE = 3;

export class Builders
{
    private static _all: Builder[] = [];

    static get all(): Builder[] { return Builders._all; }

    static initialize()
    {
        Builders._all = Creeps.ofType(CreepType.Builder).map(c => new Builder(c));

        Bodies.register(CreepType.Builder, BUILDER_MIN_SIZE, BUILDER_PARTS);
    }

    static run()
    {
        Builders._all.forEach(b => b.run());
    }

    static get served(): Set<Id<ConstructionSite>>
    {
        var ids = Builders._all.map(b => b.memory.site).filter(id => id) as Id<ConstructionSite>[];

        return new Set(ids);
    }
}