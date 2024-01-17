import * as _ from "lodash";

import { CreepBase, CreepState, CreepType, Creeps, ICreepMemory } from "./Creeps";
import { Bodies } from "./Bodies";
import { Sources } from "./Sources";
import { type } from "os";

const WELLER_PARTS: BodyPartConstant[] = [CARRY, MOVE, WORK, WORK, WORK, MOVE, WORK, WORK, WORK, MOVE, WORK, WORK, WORK, WORK];
const WELLER_MIN_SIZE = 3;

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
            case CreepState.Start: return this.prepare(CreepState.MoveToSource);
            case CreepState.MoveToSource: return this.pos.inRangeTo(source, 1) ? this.prepare(CreepState.Harvest) : state;
            case CreepState.Harvest: return this.prepareHarvest();
        }

        return state;
    }

    private prepareHarvest(): CreepState
    {
        if (this.freeEnergyCapacity < 1) return this.container ? CreepState.MoveToContainer : CreepState.Idle;

        return CreepState.Harvest;
    }
}

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

    static createMemory(source: Source): ICreepMemory
    {
        return { type: CreepType.Weller, state: CreepState.MoveToSource, source: source.id };
    }
}