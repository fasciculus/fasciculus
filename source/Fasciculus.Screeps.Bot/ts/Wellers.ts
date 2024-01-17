import * as _ from "lodash";

import { CreepBase, CreepState, CreepType, Creeps, ICreepMemory } from "./Creeps";
import { Bodies } from "./Bodies";
import { Sources } from "./Sources";

export class Weller extends CreepBase
{
    constructor(creep: Creep)
    {
        super(creep);
    }

    run()
    {
        var state = this.prepare(this.state);

        switch (state)
        {
            case CreepState.MoveToSource: this.moveTo(this.source!); break;
            case CreepState.Harvest: this.harvest(this.source!); break;
        }

        this.state = state;
    }

    private prepare(state: CreepState): CreepState
    {
        var source = this.source;

        if (!source) return this.suicide();

        switch (state)
        {
            case CreepState.Idle: return this.prepareIdle();
            case CreepState.MoveToSource: return this.prepareMoveToSource();
            case CreepState.Harvest: return this.prepareHarvest();
        }

        return state;
    }

    private prepareIdle(): CreepState
    {
        return this.freeEnergyCapacity > 0 ? this.prepareMoveToSource() : CreepState.Idle;
    }

    private prepareMoveToSource(): CreepState
    {
        return this.pos.inRangeTo(this.source!, 1) ? this.prepareHarvest() : CreepState.MoveToSource;
    }

    private prepareHarvest(): CreepState
    {
        if (this.freeEnergyCapacity < 1) return this.container ? CreepState.MoveToContainer : CreepState.Idle;

        return CreepState.Harvest;
    }
}

const WELLER_PARTS: BodyPartConstant[] = [WORK, MOVE, CARRY, WORK, WORK, MOVE, WORK, WORK, MOVE, WORK, WORK, MOVE, WORK, WORK, MOVE, WORK];
const WELLER_MIN_SIZE = 3;

export class Wellers
{
    private static _all: Weller[] = [];

    static get all(): Weller[] { return Wellers._all; }

    static initialize()
    {
        Wellers._all = Creeps.ofType(CreepType.Weller).map(c => new Weller(c));

        Bodies.register(CreepType.Weller, WELLER_MIN_SIZE, WELLER_PARTS);
    }

    static run()
    {
        Wellers._all.forEach(w => w.run());
    }

    private static sourceIdOf(weller: Weller): Id<Source>
    {
        var source = weller.source;

        return source ? source.id : ("" as Id<Source>);
    }

    static findFreeSources(): Source[]
    {
        var assigned: Set<Id<Source>> = new Set(Wellers._all.map(Wellers.sourceIdOf));

        return Sources.all.filter(s => !assigned.has(s.id));
    }
}