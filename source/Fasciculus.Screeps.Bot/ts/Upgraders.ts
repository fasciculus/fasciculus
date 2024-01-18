import { Bodies } from "./Bodies";
import { CreepBase, CreepState, Creeps, UpgraderMemory } from "./Creeps";
import { CreepType } from "./Enums";
import { Objects } from "./Objects";

export class Upgrader extends CreepBase
{
    get memory(): UpgraderMemory { return super.memory as UpgraderMemory; }

    get controller(): StructureController | undefined { return Objects.get(this.memory.controller); }

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

        if (!controller) return CreepState.Idle;

        switch (state)
        {
            case CreepState.Idle: return this.prepareIdle(controller);
            case CreepState.MoveToController: return this.prepareMoveToController(controller);
            case CreepState.Upgrade: return this.prepareUpgrade(controller);
        }

        return state;
    }

    private prepareIdle(controller: StructureController): CreepState
    {
        if (!this.inRangeTo(controller)) return CreepState.MoveToController;

        return this.energy > 0 ? CreepState.Upgrade : CreepState.Idle;
    }

    private prepareMoveToController(controller: StructureController): CreepState
    {
        return this.inRangeTo(controller) ? this.prepareIdle(controller) : CreepState.MoveToController;
    }

    private prepareUpgrade(controller: StructureController): CreepState
    {
        return this.energy > 0 ? CreepState.Upgrade : this.prepareIdle(controller);
    }

    private inRangeTo(controller: StructureController): boolean
    {
        return this.pos.inRangeTo(controller, 2)
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