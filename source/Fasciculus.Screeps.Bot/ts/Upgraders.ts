import { Bodies } from "./Bodies";
import { CreepBase, CreepType, Creeps } from "./Creeps";

export class Upgrader extends CreepBase
{
    constructor(creep: Creep)
    {
        super(creep);
    }

    run()
    {

    }
}

const UPGRADER_PARTS: BodyPartConstant[] = [WORK, MOVE, CARRY, WORK, WORK, MOVE, WORK, WORK, MOVE, WORK, WORK, MOVE, WORK, WORK, MOVE, WORK];
const UPGRADER_MIN_SIZE = 3;

export class Upgraders
{
    private static _all: Upgrader[] = [];

    static get all(): Upgrader[] { return Upgraders._all; }

    static initialize()
    {
        Upgraders._all = Creeps.ofType(CreepType.Upgrader).map(c => new Upgrader(c));

        Bodies.register(CreepType.Upgrader, UPGRADER_MIN_SIZE, UPGRADER_PARTS);
    }

    static run()
    {
        Upgraders._all.forEach(u => u.run());
    }
}