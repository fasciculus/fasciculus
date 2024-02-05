
import { CreepState, CreepType, Dictionaries, Dictionary, Stores, Vector, Vectors } from "./Common";
import { profile } from "./Profiling";
import { Mover, Positioned } from "./Travelling";

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

class BodyPartCounts
{
    readonly carry: number;
    readonly work: number;

    constructor(parts: Array<BodyPartConstant>)
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

class BodyParts
{
    static comparePriority(a: BodyPartConstant, b: BodyPartConstant)
    {
        let pa: number = BodyPartPriorities[a] || 99;
        let pb: number = BodyPartPriorities[b] || 99;

        return pa - pb;
    }

    static costOf(parts: Array<BodyPartConstant>): number
    {
        return parts.sum(p => BODYPART_COST[p]);
    }

    static countsOf(creep: Creep): BodyPartCounts
    {
        return new BodyPartCounts(creep.body.map(d => d.type));
    }

    static workOf(creep: Creep): number
    {
        return BodyParts.countsOf(creep).work;
    }
}

interface BodyPartChunk
{
    cost: number;
    parts: Array<BodyPartConstant>;
    counts: BodyPartCounts;
}

class BodyTemplate
{
    private chunks: Vector<BodyPartChunk> = new Vector();

    readonly minCost: number;

    constructor(minCost: number)
    {
        this.minCost = minCost;
    }

    static create(parts: Array<BodyPartConstant>, times: number = 1): BodyTemplate
    {
        const minCost: number = BodyParts.costOf(parts);

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

        return Vectors.flatten(this.chunks.take(chunkCount).map(c => Vector.from(c.parts))).sort(BodyParts.comparePriority);
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

        const cost = BodyParts.costOf(parts.toArray());
        const counts = new BodyPartCounts(parts.toArray());

        return { cost, parts: parts.toArray(), counts };
    }
}

export class Bodies
{
    private static _templates: Vector<Dictionary<BodyTemplate>> = new Vector();
    private static _minCost: number = 0;

    static get minCost(): number { return Bodies._minCost; }

    private static defaultBuilderTemplate = BodyTemplate.create([WORK, CARRY, MOVE, MOVE], 12);
    private static defaultRepairerTemplate = BodyTemplate.create([WORK, CARRY, MOVE, MOVE], 12);
    private static defaultUpgraderTemplate = BodyTemplate.create([WORK, CARRY, MOVE, MOVE], 12);
    private static defaultWellerTemplate = BodyTemplate.create([WORK, CARRY, MOVE]).add([WORK, MOVE]).add([WORK, CARRY, MOVE]).add([WORK, MOVE], 3);

    private static defaultGuardTemplate = BodyTemplate.create([RANGED_ATTACK, MOVE], 25);

    private static smallTankerTemplate = BodyTemplate.create([CARRY, MOVE]);
    private static largeTankerTemplate = BodyTemplate.create([CARRY, MOVE], 2);

    private static createTankerTemplate(rcl: number): BodyTemplate
    {
        return rcl < 3 ? Bodies.smallTankerTemplate : Bodies.largeTankerTemplate;
    }

    static initialize()
    {
        if (Bodies._templates.length == 0)
        {
            for (var rcl = 0; rcl < 9; ++rcl)
            {
                const templates: Dictionary<BodyTemplate> = {};

                templates[CreepType.Builder] = Bodies.defaultBuilderTemplate;
                templates[CreepType.Repairer] = Bodies.defaultRepairerTemplate;
                templates[CreepType.Tanker] = Bodies.createTankerTemplate(rcl);
                templates[CreepType.Upgrader] = Bodies.defaultUpgraderTemplate;
                templates[CreepType.Weller] = Bodies.defaultWellerTemplate;

                templates[CreepType.Guard] = Bodies.defaultGuardTemplate;

                Bodies._templates.add(templates);
            }

            const at1: Dictionary<BodyTemplate> = Bodies._templates.at(1)!;

            Bodies._minCost = Dictionaries.values(at1).map(t => t.minCost).min(c => c) || 999999;
        }
    }

    static createBody(type: CreepType, rcl: number, energy: number): Vector<BodyPartConstant> | undefined
    {
        let templates: Dictionary<BodyTemplate> | undefined = Bodies._templates.at(rcl);

        if (!templates) return undefined;

        return templates[type]?.createBody(energy);
    }
}

export interface CreepBaseMemory extends CreepMemory
{
    type: string; // CreepType;
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

    moveTo(target: Positioned, range: number, ignoreCreeps: boolean = true): CreepMoveReturnCode | ERR_NO_PATH
    {
        return Mover.moveTo(this.creep, target, range, ignoreCreeps);
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
    private static _types: Map<string, CreepType> = new Map();
    private static _creeps: Map<CreepType, Set<string>> = new Map();

    static get oldest(): Creep | undefined { return Game.myCreeps.min(c => c.ticksToLive || CREEP_LIFE_TIME); }

    @profile
    static initialize()
    {
        if (Creeps.updateTypes())
        {
            Creeps.updateCreeps();
        }
    }

    private static updateTypes(): boolean
    {
        return Creeps._types.update(Game.myCreepNames, Creeps.typeOf);
    }

    private static typeOf(name: string): CreepType
    {
        let memory = Game.creeps[name].memory as CreepBaseMemory;

        return memory.type as CreepType;
    }

    private static updateCreeps()
    {
        const creeps: Map<string, Set<string>> = Creeps._creeps;

        creeps.clear();

        Creeps._types.forEach((type, name) =>
        {
            var names: Set<string> | undefined = creeps.get(type);

            if (!names)
            {
                creeps.set(type, names = new Set());
            }

            names.add(name);
        });
    }

    static update<T>(creeps: Map<string, T>, type: CreepType, fnCreate: (name: string) => T): boolean
    {
        const names: Set<string> = Creeps._creeps.get(type) || new Set();

        return creeps.update(names, fnCreate);
    }

    @profile
    static cleanup()
    {
        const existing: Set<string> = Game.myCreepNames;

        for (const name in Memory.creeps)
        {
            if (!existing.has(name))
            {
                delete Memory.creeps[name];
            }
        }
    }
}
