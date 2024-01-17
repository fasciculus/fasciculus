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

    private static sourceIdOf(weller: Weller): Id<Source>
    {
        var source = weller.source;

        return source ? source.id : ("" as Id<Source>);
    }

    static findFreeSources(): Source[]
    {
        var assigned: Set<string> = new Set(Wellers._all.map(Wellers.sourceIdOf));

        return Sources.all.filter(s => !assigned.has(s.id));
    }

    static createMemory(source: Source): ICreepMemory
    {
        return { type: CreepType.Weller, state: CreepState.Start, source: source.id };
    }
}