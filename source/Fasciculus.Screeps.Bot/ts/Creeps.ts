
import { BodyParts, CreepState, CreepType, Dictionaries, Dictionary, GameWrap, Sets, Vector } from "./Common";
import { profile } from "./Profiling";
import { Stores } from "./Stores";

export interface CreepBaseMemory extends CreepMemory
{
    type: CreepType;
    state: CreepState;
}

export class CreepBase<M extends CreepBaseMemory>
{
    readonly name: string;

    get creep(): Creep { return Game.creeps[this.name]; }
    get memory(): M { return this.creep.memory as M; }

    get state(): CreepState { return this.memory.state; }
    set state(value: CreepState) { this.memory.state = value; }

    get id(): Id<Creep> { return this.creep.id; }
    get pos(): RoomPosition { return this.creep.pos; }

    get energy(): number { return Stores.energy(this.creep); }
    get freeEnergyCapacity(): number { return Stores.freeEnergyCapacity(this.creep); }
    get energyRatio(): number { return Stores.energyRatio(this.creep); }

    get spawning(): boolean { return this.creep.spawning; }

    readonly workParts: number;
    readonly energyCapacity: number;

    constructor(name: string)
    {
        this.name = name;

        this.workParts = BodyParts.workOf(this.creep);
        this.energyCapacity = Stores.energyCapacity(this.creep);
    }

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
    private static _types: Dictionary<CreepType> = {};
    private static _creeps: Dictionary<Set<string>> = {};

    static get oldest(): Creep | undefined { return GameWrap.myCreeps.min(c => c.ticksToLive || CREEP_LIFE_TIME); }

    @profile
    static initialize(reset: boolean)
    {
        if (reset)
        {
            Creeps._types = {};
            Creeps._creeps = {};
        }

        if (Creeps.updateTypes())
        {
            Creeps.updateCreeps();
        }
    }

    private static updateTypes(): boolean
    {
        const existing: Set<string> = Dictionaries.keys(Game.creeps);

        return Dictionaries.update(Creeps._types, existing, Creeps.typeOf);
    }

    private static typeOf(name: string): CreepType
    {
        let memory = Game.creeps[name].memory as CreepBaseMemory;

        return memory.type;
    }

    private static updateCreeps()
    {
        const types: Dictionary<CreepType> = Creeps._types
        const creeps: Dictionary<Set<string>> = {};

        for (const name of Dictionaries.keys(types))
        {
            const type = types[name];
            var names: Set<string> | undefined = creeps[type];

            if (!names)
            {
                creeps[type] = names = new Set();
            }

            names.add(name);
        }

        Creeps._creeps = creeps;
    }


    static update<T>(creeps: Dictionary<T>, type: CreepType, create: (name: string) => T): boolean
    {
        const existing: Set<string> = Creeps._creeps[type] || new Set();

        return Dictionaries.update(creeps, existing, create);
    }

    @profile
    static cleanup()
    {
        const existing: Set<string> = Dictionaries.keys(Game.creeps);

        for (const name in Memory.creeps)
        {
            if (!existing.has(name))
            {
                delete Memory.creeps[name];
            }
        }
    }
}
