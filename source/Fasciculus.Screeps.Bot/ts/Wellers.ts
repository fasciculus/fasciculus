import { CreepType, Creeps, ICreepMemory } from "./Creeps";
import { Objects } from "./Objects";

export enum WellerState
{
    MoveToSource,
    Harvest,
    MoveToContainer,
    Idle
}

export interface IWellerMemory extends ICreepMemory
{
    source: Id<Source>;

    container?: Id<StructureContainer>;

    state: WellerState;
}

export class Weller
{
    readonly creep: Creep;

    get memory(): IWellerMemory { return this.creep.memory as IWellerMemory; }

    get source(): Source | null { return Objects.source(this.memory.source); }
    get container(): StructureContainer | null { return Objects.container(this.memory.container); }
    get state(): WellerState { return this.memory.state; }

    constructor(creep: Creep)
    {
        this.creep = creep;
    }
}

export class Wellers
{
    private static _all: Weller[] = [];

    static get all(): Weller[] { return Wellers._all; }

    static initialize()
    {
        Wellers._all = Creeps.ofType(CreepType.Weller).map(c => new Weller(c));
    }
}