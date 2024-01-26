
import { CreepBaseMemory } from "./Memories";
import { GameWrap } from "./GameWrap";
import { Stores } from "./Stores";
import { BodyParts } from "./Bodies";
import { Dictionary, Vector } from "./Collections";
import { CreepState, CreepType } from "./Types";

export class CreepBase
{
    readonly creep: Creep;

    readonly workParts: number;

    private _state: CreepState;

    constructor(creep: Creep)
    {
        this.creep = creep;
        this.workParts = BodyParts.workOf(creep);
        this._state = this.memory.state;
    }

    get memory(): CreepBaseMemory { return this.creep.memory as CreepBaseMemory; }

    get state(): CreepState { return this._state; }
    set state(value: CreepState) { this._state = value; this.memory.state = value; }

    get id(): Id<Creep> { return this.creep.id; }
    get name(): string { return this.creep.name; }
    get pos(): RoomPosition { return this.creep.pos; }
    get store(): StoreDefinition { return this.creep.store; }

    get energy(): number { return Stores.energy(this); }
    get energyCapacity(): number { return Stores.energyCapacity(this); }
    get freeEnergyCapacity(): number { return Stores.freeEnergyCapacity(this); }
    get energyRatio(): number { return Stores.energyRatio(this); }

    get spawning(): boolean { return this.creep.spawning; }

    moveTo(target: RoomPosition | { pos: RoomPosition }, opts?: MoveToOpts): CreepMoveReturnCode | ERR_NO_PATH | ERR_INVALID_TARGET | ERR_NOT_FOUND
    {
        let creep = this.creep;

        if (creep.fatigue > 0) return ERR_TIRED;

        return creep.moveTo(target, opts);
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

    repair(target: Structure)
    {
        return this.creep.repair(target);
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

export class Creeps
{
    private static _my: Vector<Creep> = new Vector();
    private static _ofType: Dictionary<Vector<Creep>> = {};

    static get(name: string): Creep | undefined { return GameWrap.myCreep(name); }

    static get my(): Vector<Creep> { return Creeps._my.clone(); }
    static get oldest(): Creep | undefined { return Creeps._my.min(c => c.ticksToLive || CREEP_LIFE_TIME); }

    static ofType(type: CreepType): Vector<Creep> { return Creeps._ofType[type] || new Vector(); }
    static countOf(type: CreepType): number { return Creeps.ofType(type).length; }

    static typeOf(creep: Creep): CreepType
    {
        let memory = creep.memory as CreepBaseMemory;

        return memory.type;
    }

    static initialize()
    {
        Creeps._my = GameWrap.myCreeps;
        Creeps._ofType = Creeps._my.groupBy(Creeps.typeOf);
    }

    static resetStates()
    {
        Creeps._my.forEach(c => (c.memory as CreepBaseMemory).state = CreepState.Idle);
    }
}