import { Bodies, BodyTemplate } from "./Bodies";
import { CreepState, CreepType, Dictionaries, Dictionary, Repairable, RepairableId, Vector } from "./Common";
import { CreepBase, CreepBaseMemory, Creeps } from "./Creeps";
import { Positions } from "./Positions";
import { profile } from "./Profiling";
import { Repairs } from "./Repairs";
import { Walls } from "./Walls";

const REPAIRER_TEMPLATE: BodyTemplate = BodyTemplate.create([WORK, CARRY, MOVE, MOVE], 12);

const REPAIRER_MOVE_TO_OPTS: MoveToOpts =
{
    visualizePathStyle:
    {
        stroke: "#0f0"
    }
};

class DamageHelper
{
    static damageOf(repairable: Repairable): number
    {
        let hitsMax = repairable instanceof StructureWall ? Walls.avg + 2000 : repairable.hitsMax;

        return Math.max(0, hitsMax - repairable.hits);
    }
}

interface RepairerMemory extends CreepBaseMemory
{
    repairable?: RepairableId;
}

export class Repairer extends CreepBase<RepairerMemory>
{
    get repairable(): Repairable | undefined { return Repairs.get(this.memory.repairable); }
    set repairable(value: Repairable | undefined) { this.memory.repairable = value?.id; }

    readonly maxEnergyPerTick: number;

    constructor(creep: Creep)
    {
        super(creep, CreepType.Repairer);

        this.maxEnergyPerTick = this.workParts;
    }

    execute()
    {
        switch (this.state)
        {
            case CreepState.ToRepairable: this.executeToRepair(); break;
            case CreepState.Repair: this.executeRepair(); break;
        }
    }

    private executeToRepair()
    {
        let repairable = this.repairable;

        if (!repairable) return;

        this.moveTo(repairable, REPAIRER_MOVE_TO_OPTS);
    }

    private executeRepair()
    {
        let repairable = this.repairable;

        if (!repairable) return;

        this.repair(repairable);
    }

    prepare()
    {
        switch (this.state)
        {
            case CreepState.Idle: this.state = this.prepareIdle(); break;
            case CreepState.ToRepairable: this.state = this.prepareToRepairable(); break;
            case CreepState.Repair: this.state = this.prepareRepair(); break;
        }
    }

    private prepareIdle(): CreepState
    {
        let repairable = this.repairable;

        if (!repairable) return CreepState.Idle;

        return this.inRangeTo(repairable) ? CreepState.Repair : CreepState.ToRepairable;
    }

    private prepareToRepairable(): CreepState
    {
        let repairable = this.repairable;

        if (!repairable) return CreepState.Idle;

        return this.inRangeTo(repairable) ? CreepState.Repair : CreepState.ToRepairable;
    }

    private prepareRepair(): CreepState
    {
        let repairable = this.repairable;

        if (!repairable) return CreepState.Idle;

        if (DamageHelper.damageOf(repairable) == 0)
        {
            this.repairable = undefined;
            return CreepState.Idle;
        }

        return CreepState.Repair;
    }

    private inRangeTo(repairable: Repairable)
    {
        return this.pos.inRangeTo(repairable, 2);
    }
}
 
interface DamageInfo
{
    repairable: Repairable;
    damage: number;
}

export class Repairers
{
    private static _all: Vector<Repairer> = new Vector();

    static get all(): Vector<Repairer> { return Repairers._all.clone(); }

    static get maxEnergyPerTick(): number { return Repairers._all.sum(r => r.maxEnergyPerTick); }

    static initialize()
    {
        Repairers._all = Creeps.ofType(CreepType.Repairer).map(c => new Repairer(c));

        Bodies.register(CreepType.Repairer, REPAIRER_TEMPLATE);
    }

    @profile
    static run()
    {
        Repairers._all.forEach(r => r.prepare());
        Repairers.assign().forEach(r => r.prepare());
        Repairers._all.forEach(r => r.execute());
    }

    private static assign(): Vector<Repairer>
    {
        var result: Vector<Repairer> = new Vector();
        var unassigned: Dictionary<Repairer> = Repairers.unassignedRepairers;

        if (Dictionaries.isEmpty(unassigned)) return result;

        for (let repairable of Repairers.repairables)
        {
            let assignables: Vector<Repairer> = Dictionaries.values(unassigned);
            let repairer: Repairer | undefined = Positions.closestByPath(repairable, assignables);

            if (!repairer) continue;

            repairer.repairable = repairable;
            result.append(repairer);
            delete unassigned[repairer.name];

            if (Dictionaries.isEmpty(unassigned)) break;
        }

        return result;
    }

    private static get unassignedRepairers(): Dictionary<Repairer>
    {
        return Repairers._all.filter(r => !r.repairable).indexBy(r => r.name);
    }

    private static get repairables(): Vector<Repairable>
    {
        return Repairs.all.map(Repairers.toDamageInfo).sort(Repairers.compareDamage).map(d => d.repairable);
    }

    private static toDamageInfo(repairable: Repairable): DamageInfo
    {
        let result: DamageInfo = { repairable, damage: DamageHelper.damageOf(repairable) };

        return result;
    }

    private static compareDamage(a: DamageInfo, b: DamageInfo): number
    {
        return b.damage - a.damage;
    }
}