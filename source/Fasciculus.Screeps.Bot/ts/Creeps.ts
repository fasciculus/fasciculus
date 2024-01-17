
import * as _ from "lodash";
import { Objects } from "./Objects";

export enum CreepType
{
    Weller = "W",
    Supplier = "S"
}

export enum CreepState
{
    Start,
    MoveToSource,
    MoveToContainer,
    Harvest,
    Idle,
    Suicide
}

export const CreepStateText: string[] =
    [
        "Start",
        "→Source",
        "→Container",
        "Harvest",
        "Idle",
        "Suicide"
    ];

export interface ICreepMemory extends CreepMemory
{
    type: CreepType;
    state: CreepState;

    container?: Id<StructureContainer>;
    source?: Id<Source>;
}

export class CreepBase
{
    readonly creep: Creep;

    constructor(creep: Creep)
    {
        this.creep = creep;
    }

    get memory(): ICreepMemory { return this.creep.memory as ICreepMemory; }

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

    get container(): StructureContainer | null { return Objects.container(this.memory.container); }
    get source(): Source | null { return Objects.source(this.memory.source); }

    get name(): string { return this.creep.name; }
    get pos(): RoomPosition { return this.creep.pos; }

    get store(): StoreDefinition { return this.creep.store; }
    get freeEnergyCapacity(): number { return this.store.getFreeCapacity<RESOURCE_ENERGY>() }

    moveTo(target: RoomPosition | { pos: RoomPosition }): CreepMoveReturnCode | ERR_NO_PATH | ERR_INVALID_TARGET | ERR_NOT_FOUND
    {
        return this.creep.moveTo(target);
    }

    harvest(target: Source | Mineral | Deposit): CreepActionReturnCode | ERR_NOT_FOUND | ERR_NOT_ENOUGH_RESOURCES
    {
        return this.creep.harvest(target);
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
    private static _all: Creep[] = [];
    private static _my: Creep[] = [];
    private static _ofType: _.Dictionary<Creep[]> = {};

    static get all(): Creep[] { return Creeps._all; }
    static get my(): Creep[] { return Creeps._my; }

    static ofType(type: CreepType): Creep[] { return Creeps._ofType[type] || []; }

    static typeOf(creep: Creep): CreepType
    {
        let memory = creep.memory as ICreepMemory;

        return memory.type;
    }

    static initialize()
    {
        Creeps._all = _.values(Game.creeps);
        Creeps._my = Creeps._all.filter(c => c.my);
        Creeps._ofType = _.groupBy(Creeps._my, Creeps.typeOf);
    }
}