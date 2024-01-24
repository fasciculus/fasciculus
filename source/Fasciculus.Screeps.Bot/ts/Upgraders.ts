import * as _ from "lodash";
import { Bodies, BodyTemplate } from "./Bodies";
import { Controller, Controllers } from "./Controllers";
import { CreepBase, Creeps } from "./Creeps";
import { CreepState, CreepType } from "./Enums";
import { UpgraderMemory } from "./Memories";

const UPGRADER_TEMPLATE: BodyTemplate = BodyTemplate.create([WORK, CARRY, MOVE])
    .add([WORK, MOVE]).add([WORK, CARRY, MOVE]).add([WORK, MOVE]).add([WORK, CARRY, MOVE])
    .add([WORK, MOVE]).add([WORK, CARRY, MOVE]).add([WORK, MOVE]).add([WORK, CARRY, MOVE])
    .add([WORK, MOVE]).add([WORK, CARRY, MOVE]).add([WORK, MOVE]).add([WORK, CARRY, MOVE])
    .add([WORK, MOVE]).add([WORK, CARRY, MOVE]).add([WORK, MOVE]).add([WORK, CARRY, MOVE])
    .add([WORK, MOVE]).add([WORK, CARRY, MOVE]).add([WORK, MOVE]);

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
    private static _all: Upgrader[] = [];

    static get all(): Upgrader[] { return Upgraders._all; }

    static get maxEnergyPerTick(): number { return _.sum(Upgraders._all.map(u => u.maxEnergyPerTick)); }

    static initialize()
    {
        Upgraders._all = Creeps.ofType(CreepType.Upgrader).map(c => new Upgrader(c)).values;

        Bodies.register(CreepType.Upgrader, UPGRADER_TEMPLATE);
    }

    static run()
    {
        Upgraders._all.forEach(u => u.prepare());

        var newlyAssigned: Upgrader[]  = Upgraders.assign();

        newlyAssigned.forEach(u => u.prepare());
        Upgraders._all.forEach(u => u.execute());
    }

    private static assign(): Upgrader[]
    {
        let unassignedUpgraders = Upgraders._all.filter(u => !u.controller);

        if (unassignedUpgraders.length == 0) return [];

        let works = Upgraders.getControllerWorks();

        if (works.length == 0) return [];

        let sorted = works.sort((a, b) => a.work - b.work);
        let controller = sorted[0].controller;
        let upgrader = controller.pos.findClosestByPath(unassignedUpgraders) || undefined;

        if (!upgrader) return [];

        upgrader.controller = controller;

        return [upgrader];
    }

    private static getControllerWorks(): ControllerWork[]
    {
        let result: ControllerWork[] = [];

        Controllers.my.forEach(c => result.push({ controller: c, work: 0 }));

        let byId = _.indexBy(result, cw => cw.controller.id);

        for (let upgrader of Upgraders._all)
        {
            let controller = upgrader.controller;

            if (!controller) continue;

            byId[controller.id].work += upgrader.workParts;
        }

        return result;
    }
}