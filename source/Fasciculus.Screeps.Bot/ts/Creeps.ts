
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

    static ofType(type: CreepType): Vector<Creep> { return GameWrap.myCreeps.filter(c => Creeps._types[c.name] == type); }

    static get oldest(): Creep | undefined { return GameWrap.myCreeps.min(c => c.ticksToLive || CREEP_LIFE_TIME); }

    @profile
    static initialize(clear: boolean)
    {
        Creeps.clear(clear);
        Creeps.updateTypes();
    }

    private static clear(clear: boolean)
    {
        if (clear)
        {
            Creeps._types = {};
        }
    }

    private static typeOf(creep: Creep): CreepType
    {
        let memory = creep.memory as CreepBaseMemory;

        return memory.type;
    }

    private static updateTypes()
    {
        const existing: Set<string> = Dictionaries.keys(Game.creeps);
        const toDelete: Set<string> = Sets.difference(Dictionaries.keys(Creeps._types), existing);
        const toCreate: Set<string> = Sets.difference(existing, Dictionaries.keys(Creeps._types));

        Dictionaries.removeAll(Creeps._types, toDelete);

        for (const name of toCreate)
        {
            Creeps._types[name] = Creeps.typeOf(Game.creeps[name]);
        }
    }

    static update<T>(creeps: Dictionary<T>, type: CreepType, create: (name: string) => T): boolean
    {
        const existing: Set<string> = CreepTypes.creepsOfType(type);

        return Dictionaries.update(creeps, existing, create);
    }
}

export class CreepMemories
{
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

export class CreepTypes
{
    private static _types: Dictionary<CreepType> = {};
    private static _creeps: Dictionary<Set<string>> = {};

    static creepsOfType(type: CreepType): Set<string>
    {
        const creeps: Set<string> = CreepTypes._creeps[type] || new Set();

        return Sets.clone(creeps);
    }

    private static clear(clear: boolean)
    {
        if (clear)
        {
            CreepTypes._types = {};
            CreepTypes._creeps = {};
        }
    }

    @profile
    static initialize(clear: boolean)
    {
        CreepTypes.clear(clear);

        if (CreepTypes.updateTypes())
        {
            CreepTypes.updateCreeps();
        }
    }

    private static updateTypes(): boolean
    {
        const existing: Set<string> = Dictionaries.keys(Game.creeps);

        return Dictionaries.update(CreepTypes._types, existing, CreepTypes.typeOf);
    }

    private static typeOf(name: string): CreepType
    {
        let memory = Game.creeps[name].memory as CreepBaseMemory;

        return memory.type;
    }

    private static updateCreeps()
    {
        const types: Dictionary<CreepType> = CreepTypes._types
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

        CreepTypes._creeps = creeps;
    }
}