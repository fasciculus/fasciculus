
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
    Idle
}

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

    get container(): StructureContainer | null { return Objects.container(this.memory.container); }
    get source(): Source | null { return Objects.source(this.memory.source); }
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