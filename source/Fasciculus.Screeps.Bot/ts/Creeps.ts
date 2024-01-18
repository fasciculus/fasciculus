
import * as _ from "lodash";
import { CreepState, CreepType } from "./Enums";
import { CreepBaseMemory } from "./Memories";
import { GameWrap } from "./GameWrap";

export const CreepStateText: string[] =
    [
        "Idle",
        "Suicide",
        "> Containr",
        "> Contrllr",
        "> Customer",
        "> Site",
        "> Source",
        "> Supply",
        "Harvest",
        "Withdraw",
        "Transfer",
        "Upgrade",
        "Build"
    ];

export class CreepBase
{
    readonly creep: Creep;

    constructor(creep: Creep)
    {
        this.creep = creep;
    }

    get memory(): CreepBaseMemory { return this.creep.memory as CreepBaseMemory; }

    get state(): CreepState { return this.memory.state; }

    set state(value: CreepState)
    {
        var oldValue = this.memory.state;

        if (value != oldValue)
        {
            this.memory.state = value;
            this.say(CreepStateText[value]);
        }
    }

    get id(): Id<Creep> { return this.creep.id; }
    get name(): string { return this.creep.name; }
    get pos(): RoomPosition { return this.creep.pos; }

    get store(): StoreDefinition { return this.creep.store; }
    get energy(): number { return this.store.energy; }
    get freeEnergyCapacity(): number { return this.store.getFreeCapacity<RESOURCE_ENERGY>() }

    moveTo(target: RoomPosition | { pos: RoomPosition }, opts?: MoveToOpts): CreepMoveReturnCode | ERR_NO_PATH | ERR_INVALID_TARGET | ERR_NOT_FOUND
    {
        return this.creep.moveTo(target, opts);
    }

    build(target: ConstructionSite): CreepActionReturnCode | ERR_NOT_ENOUGH_RESOURCES | ERR_RCL_NOT_ENOUGH
    {
        return this.creep.build(target);
    }

    harvest(target: Source | Mineral | Deposit): CreepActionReturnCode | ERR_NOT_FOUND | ERR_NOT_ENOUGH_RESOURCES
    {
        return this.creep.harvest(target);
    }

    transfer(target: AnyCreep | Structure, resourceType: ResourceConstant, amount?: number): ScreepsReturnCode
    {
        return this.creep.transfer(target, resourceType, amount);
    }

    upgradeController(target: StructureController): ScreepsReturnCode
    {
        return this.creep.upgradeController(target);
    }

    withdraw(target: Structure | Tombstone | Ruin | Creep, resourceType: ResourceConstant, amount?: number): ScreepsReturnCode
    {
        if (target instanceof Creep)
        {
            return target.transfer(this.creep, resourceType, amount);
        }
        else
        {
            return this.creep.withdraw(target, resourceType, amount);
        }
    }

    say(message: string, toPublic?: boolean)
    {
        this.creep.say(message, toPublic);
    }

    suicide(): CreepState
    {
        return this.creep.suicide() == OK ? CreepState.Suicide : this.state;
    }
}

export interface CreepTemplate
{
    minBodyParts: number;
    parts: BodyPartConstant[];
}

export class Creeps
{
    private static _my: Creep[] = [];
    private static _ofType: _.Dictionary<Creep[]> = {};

    static get my(): Creep[] { return Creeps._my; }

    static ofType(type: CreepType): Creep[] { return Creeps._ofType[type] || []; }
    static countOf(type: CreepType): number { return Creeps.ofType(type).length; }

    static typeOf(creep: Creep): CreepType
    {
        let memory = creep.memory as CreepBaseMemory;

        return memory.type;
    }

    static initialize()
    {
        Creeps._my = GameWrap.myCreeps;
        Creeps._ofType = _.groupBy(Creeps._my, Creeps.typeOf);
    }

    static resetStates()
    {
        Creeps._my.forEach(c => (c.memory as CreepBaseMemory).state = CreepState.Idle);
    }
}