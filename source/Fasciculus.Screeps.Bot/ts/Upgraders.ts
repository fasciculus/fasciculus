import { Bodies } from "./Bodies";
import { CreepBase, CreepState, CreepType, Creeps } from "./Creeps";

export class Upgrader extends CreepBase
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
            case CreepState.MoveToController: this.moveTo(this.controller!); break;
            case CreepState.Upgrade: this.upgradeController(this.controller!); break;
        }

        this.state = state;
    }

    private prepare(state: CreepState): CreepState
    {
        var controller = this.controller;

        if (!controller || !controller.my) return this.suicide();

        switch (state)
        {
            case CreepState.Idle: return this.prepareIdle(controller);
            case CreepState.MoveToController: return this.prepareMoveToController(controller);
        }

        return state;
    }

    private prepareIdle(controller: StructureController): CreepState
    {
        return this.pos.inRangeTo(controller, 2) ? CreepState.Upgrade : CreepState.MoveToController;
    }

    private prepareMoveToController(controller: StructureController): CreepState
    {
        return this.pos.inRangeTo(controller, 2) ? CreepState.Upgrade : CreepState.MoveToController;
    }
}

const UPGRADER_PARTS: BodyPartConstant[] = [WORK, MOVE, CARRY, WORK, WORK, MOVE, WORK, WORK];
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