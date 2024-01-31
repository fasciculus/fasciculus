
import { CreepState, CreepType, Dictionaries, Dictionary, GameWrap, Sets, Vector, Vectors } from "./Common";
import { profile } from "./Profiling";
import { Stores } from "./Stores";

const BodyPartPriorities =
{
    "tough": 1,
    "work": 2,
    "attack": 3,
    "ranged_attack": 4,
    "carry": 5,
    "move": 6,
    "heal": 7,
    "claim": 8
}

export class BodyPartCounts
{
    readonly carry: number;
    readonly work: number;

    constructor(parts: Vector<BodyPartConstant>)
    {
        var carry: number = 0;
        var work: number = 0;

        for (let part of parts)
        {
            switch (part)
            {
                case CARRY: ++carry; break;
                case WORK: ++work; break;
            }
        }

        this.carry = carry;
        this.work = work;
    }
}

export class BodyParts
{
    static comparePriority(a: BodyPartConstant, b: BodyPartConstant)
    {
        let pa: number = BodyPartPriorities[a] || 99;
        let pb: number = BodyPartPriorities[b] || 99;

        return pa - pb;
    }

    static costOf(parts: Vector<BodyPartConstant>): number
    {
        return parts.sum(p => BODYPART_COST[p]);
    }

    static countsOf(creep: Creep): BodyPartCounts
    {
        const definitions: Vector<BodyPartDefinition> = Vector.from(creep.body);
        const parts: Vector<BodyPartConstant> = definitions.map(d => d.type);

        return new BodyPartCounts(parts);
    }

    static workOf(creep: Creep): number
    {
        return BodyParts.countsOf(creep).work;
    }
}

interface BodyPartChunk
{
    cost: number;
    parts: Vector<BodyPartConstant>;
    counts: BodyPartCounts;
}

export class BodyTemplate
{
    private chunks: Vector<BodyPartChunk> = new Vector();

    readonly minCost: number;

    constructor(minCost: number)
    {
        this.minCost = minCost;
    }

    static create(parts: BodyPartConstant[], times: number = 1): BodyTemplate
    {
        const minCost: number = BodyParts.costOf(Vector.from(parts));

        return new BodyTemplate(minCost).add(parts, times);
    }

    add(parts: BodyPartConstant[], times: number = 1): BodyTemplate
    {
        let chunk: BodyPartChunk | undefined = BodyTemplate.createChunk(Vector.from(parts));

        if (!chunk) return this;

        for (let i = 0; i < times; ++i)
        {
            this.chunks.add(chunk);
        }

        return this;
    }

    createBody(energy: number): Vector<BodyPartConstant> | undefined
    {
        let chunkCount = this.chunkCount(energy);

        if (chunkCount == 0) return undefined;

        return Vectors.flatten(this.chunks.take(chunkCount).map(c => c.parts)).sort(BodyParts.comparePriority);
    }

    private chunkCount(energy: number): number
    {
        var result: number = 0;

        for (let chunk of this.chunks)
        {
            if (energy < chunk.cost) break;

            ++result;
            energy -= chunk.cost;
        }

        return result;
    }

    private static createChunk(parts: Vector<BodyPartConstant>): BodyPartChunk | undefined
    {
        if (parts.length == 0) return undefined;

        const cost = BodyParts.costOf(parts);
        const counts = new BodyPartCounts(parts);

        return { cost, parts, counts };
    }
}

export class Bodies
{
    private static _templates: Dictionary<BodyTemplate> = {};
    private static _minCost: number = 0;

    static get minCost(): number { return Bodies._minCost; }

    static initialize(reset: boolean)
    {
        if (reset)
        {
            Bodies._templates = {};
        }

        if (Dictionaries.isEmpty(Bodies._templates))
        {
            Bodies._templates[CreepType.Builder] = Bodies.createBuilderTemplate();
            Bodies._templates[CreepType.Repairer] = Bodies.createRepairerTemplate();
            Bodies._templates[CreepType.Upgrader] = Bodies.createUpgraderTemplate();
            Bodies._templates[CreepType.Weller] = Bodies.createWellerTemplate();

            Bodies._minCost = Dictionaries.values(Bodies._templates).map(t => t.minCost).min(c => c) || 999999;
        }
    }

    static createBody(type: CreepType, energy: number): Vector<BodyPartConstant> | undefined
    {
        return Bodies._templates[type]?.createBody(energy);
    }

    private static createBuilderTemplate(): BodyTemplate
    {
        return BodyTemplate.create([WORK, CARRY, MOVE, MOVE], 12);
    }

    private static createRepairerTemplate(): BodyTemplate
    {
        return BodyTemplate.create([WORK, CARRY, MOVE, MOVE], 12);
    }

    private static createUpgraderTemplate(): BodyTemplate
    {
        return BodyTemplate.create([WORK, CARRY, MOVE, MOVE], 12);
    }

    private static createWellerTemplate(): BodyTemplate
    {
        return BodyTemplate.create([WORK, CARRY, MOVE]).add([WORK, MOVE]).add([WORK, CARRY, MOVE]).add([WORK, MOVE], 3);
    }
}

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
