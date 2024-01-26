import { Bodies, BodyTemplate } from "./Bodies";
import { Controller, Controllers } from "./Controllers";
import { CreepBase, Creeps } from "./Creeps";
import { CreepState, CreepType } from "./Enums";
import { UpgraderMemory } from "./Memories";
import { Dictionary, Vector } from "./Collections";
import { Positions } from "./Positions";
import { profile } from "./Profiling";

const UPGRADER_TEMPLATE: BodyTemplate = BodyTemplate.create([WORK, CARRY, MOVE])
    .add([WORK, CARRY, MOVE]).add([WORK, MOVE]).add([WORK, CARRY, MOVE]).add([WORK, MOVE])
    .add([WORK, CARRY, MOVE]).add([WORK, MOVE]).add([WORK, CARRY, MOVE]).add([WORK, MOVE])
    .add([WORK, CARRY, MOVE]).add([WORK, MOVE]).add([WORK, CARRY, MOVE]).add([WORK, MOVE])
    .add([WORK, CARRY, MOVE]).add([WORK, MOVE]).add([WORK, CARRY, MOVE]).add([WORK, MOVE])
    .add([WORK, CARRY, MOVE]).add([WORK, MOVE]).add([WORK, MOVE]);

export class Upgrader extends CreepBase
{
    readonly maxEnergyPerTick: number;

    get memory(): UpgraderMemory { return super.memory as UpgraderMemory; }

    get controller(): Controller | undefined { return Controllers.get(this.memory.controller); }
    set controller(value: Controller | undefined) { this.memory.controller = value?.id; }

    constructor(creep: Creep)
    {
        super(creep);

        this.maxEnergyPerTick = this.workParts;
    }

    execute()
    {
        switch (this.state)
        {
            case CreepState.ToController: this.executeToController(); break;
            case CreepState.Upgrade: this.executeUpgrade(); break;
        }
    }

    private executeToController()
    {
        let controller = this.controller;

        if (!controller) return;

        this.moveTo(controller);
    }

    private executeUpgrade()
    {
        let controller = this.controller;

        if (!controller) return;

        this.upgradeController(controller.controller);
    }

    prepare()
    {
        switch (this.state)
        {
            case CreepState.Idle: this.state = this.prepareIdle();
            case CreepState.ToController: this.state = this.prepareToController();
            case CreepState.Upgrade: this.state = this.prepareUpgrade();
        }
    }

    private prepareIdle(): CreepState
    {
        let controller = this.controller;

        if (!controller) return CreepState.Idle;

        return this.inRangeTo(controller) ? CreepState.Upgrade : CreepState.ToController;
    }

    private prepareToController(): CreepState
    {
        let controller = this.controller;

        if (!controller) return CreepState.Idle;

        return this.inRangeTo(controller) ? CreepState.Upgrade : CreepState.ToController;
    }

    private prepareUpgrade(): CreepState
    {
        let controller = this.controller;

        if (!controller) return CreepState.Idle;

        return this.inRangeTo(controller) ? CreepState.Upgrade : CreepState.ToController;
    }

    private inRangeTo(controller: Controller): boolean
    {
        return this.pos.inRangeTo(controller, 2)
    }
}

interface ControllerWork
{
    controller: Controller;
    work: number;
}

export class Upgraders
{
    private static _all: Vector<Upgrader> = new Vector();

    static get count(): number { return Upgraders._all.length; }
    static get maxEnergyPerTick(): number { return Upgraders._all.sum(u => u.maxEnergyPerTick); }

    static initialize()
    {
        Upgraders._all = Creeps.ofType(CreepType.Upgrader).map(c => new Upgrader(c));

        Bodies.register(CreepType.Upgrader, UPGRADER_TEMPLATE);
    }

    static run()
    {
        Upgraders.prepare(Upgraders._all);
        Upgraders.prepare(Upgraders.assign());
        Upgraders.execute(Upgraders._all);
    }

    @profile
    private static prepare(upgraders: Vector<Upgrader>)
    {
        upgraders.forEach(u => u.prepare());
    }

    @profile
    private static execute(upgraders: Vector<Upgrader>)
    {
        upgraders.forEach(u => u.execute());
    }

    private static assign(): Vector<Upgrader>
    {
        var result: Vector<Upgrader> = new Vector();
        let unassigned: Vector<Upgrader> = Upgraders._all.filter(u => !u.controller);

        if (unassigned.length == 0) return result;

        let works: Vector<ControllerWork> = Upgraders.getControllerWorks();

        if (works.length == 0) return result;

        let sorted: Vector<ControllerWork> = works.sort((a, b) => a.work - b.work);
        let controller: Controller | undefined = sorted.at(0)?.controller;

        if (!controller) return result;

        let upgrader: Upgrader | undefined = Positions.closestByPath(controller, unassigned);

        if (!upgrader) return result;

        upgrader.controller = controller;
        result.append(upgrader);

        return result;
    }

    private static getControllerWorks(): Vector<ControllerWork>
    {
        let result: Vector<ControllerWork> = Controllers.my.map(Upgraders.createControllerWork);
        let byId: Dictionary<ControllerWork> = result.indexBy(cw => cw.controller.id);

        for (let upgrader of Upgraders._all)
        {
            let controller = upgrader.controller;

            if (!controller) continue;

            byId[controller.id].work += upgrader.workParts;
        }

        return result;
    }

    private static createControllerWork(controller: Controller): ControllerWork
    {
        var result: ControllerWork = { controller, work: 0 };

        return result;
    }
}