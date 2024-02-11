
export class Assigns
{
    private static _assigns: Map<AssignableId, Set<CreepId>> = new Map();

    static assignees(target: AssignableId): Set<CreepId>
    {
        return Assigns._assigns.ensure(target, () => new Set());
    }

    static assign(target: AssignableId, creep: CreepId): void
    {
        Assigns.assignees(target).add(creep);
    }

    static unassign(target: AssignableId, creep: CreepId): void
    {
        Assigns.assignees(target).delete(creep);
    }

    static unassignAll(target: AssignableId): void
    {
        Assigns._assigns.delete(target);
    }

    static cleanup()
    {
        const assigns: Map<AssignableId, Set<CreepId>> = Assigns._assigns;

        assigns.keep(Game.existing(assigns.ids));
        assigns.forEach(creeps => creeps.keep(Game.existing(creeps)));
    }
}