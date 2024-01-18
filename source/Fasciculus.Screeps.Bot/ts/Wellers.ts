import * as _ from "lodash";

import { CreepBase, CreepState, Creeps, WellerMemory } from "./Creeps";
import { Bodies } from "./Bodies";
import { Sources } from "./Sources";
import { Objects } from "./Objects";
import { CreepType } from "./Enums";

export class Weller extends CreepBase
{
    get memory(): WellerMemory { return super.memory as WellerMemory; }

    get source(): Source | undefined { return Objects.get(this.memory.source); }

    constructor(creep: Creep)
    {
        super(creep);
    }

    run()
    {
        var state = this.prepare(this.state);
        var source = this.source;

        if (!source) return;

        switch (state)
        {
            case CreepState.MoveToSource: this.moveTo(source); break;
            case CreepState.Harvest: this.harvest(source); break;
        }

        this.state = state;
    }

    private prepare(state: CreepState): CreepState
    {
        var source = this.source;

        if (!source) return CreepState.Idle;

        switch (state)
        {
            case CreepState.Idle: return this.prepareIdle(source);
            case CreepState.MoveToSource: return this.prepareMoveToSource(source);
            case CreepState.Harvest: return this.prepareHarvest(source);
        }

        return state;
    }

    private prepareIdle(source: Source): CreepState
    {
        if (this.freeEnergyCapacity > 0)
        {
            return this.inRangeTo(source) ? CreepState.Harvest : CreepState.MoveToSource;
        }
        else
        {
            return CreepState.Idle;
        }
    }

    private prepareMoveToSource(source: Source): CreepState
    {
        return this.inRangeTo(source) ? this.prepareIdle(source) : CreepState.MoveToSource;
    }

    private prepareHarvest(source: Source): CreepState
    {
        return this.freeEnergyCapacity == 0 ? this.prepareIdle(source) : CreepState.Harvest;
    }

    private inRangeTo(target: Source | StructureContainer | null): boolean
    {
        if (!target) return false;

        return this.pos.inRangeTo(target, 1);
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