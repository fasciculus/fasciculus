import { CreepType, Dictionaries, Dictionary, Positions, SerializedPosition, Vector } from "./Common";
import { CreepBase, CreepBaseMemory, Creeps } from "./Creeps";
import { Markers } from "./Markers";
import { profile } from "./Profiling";

interface GuardMemory extends CreepBaseMemory
{
    sentry?: SerializedPosition;
    guarding?: boolean;
}

export class Guard extends CreepBase<GuardMemory>
{
    get sentry(): RoomPosition | undefined { return Positions.deserialize(this.memory.sentry); }
    set sentry(value: RoomPosition | undefined) { this.memory.sentry = Positions.serialize(value); }

    private get guarding(): boolean { return this.memory.guarding || false; }
    private set guarding(value: boolean) { this.memory.guarding = value; }

    constructor(name: string)
    {
        super(name);
    }

    prepare()
    {
    }

    execute()
    {
    }
}

export class Guards
{
    private static _guards: Dictionary<Guard> = {};

    private static _all: Vector<Guard> = new Vector();

    static get count(): number { return Guards._all.length; }

    @profile
    static initialize(reset: boolean)
    {
        if (reset)
        {
            Guards._guards = {};
            Guards._all = new Vector();
        }

        if (Creeps.update(Guards._guards, CreepType.Guard, name => new Guard(name)))
        {
            Guards._all = Dictionaries.values(Guards._guards);
        }
    }

    @profile
    static run()
    {
        Guards.prepare(Guards._all);
        Guards.prepare(Guards.assign());
        Guards.execute(Guards._all);
    }

    @profile
    private static prepare(guards: Vector<Guard>)
    {
        guards.forEach(b => b.prepare());
    }

    @profile
    private static execute(guards: Vector<Guard>)
    {
        guards.forEach(b => b.execute());
    }

    @profile
    private static assign(): Vector<Guard>
    {
        const result: Vector<Guard> = new Vector();

        if (Markers.guardMarkerCount == 0) return result;

        const unassignedGuards = Guards._all.filter(g => !g.sentry);

        if (unassignedGuards.length == 0) return result;

        const unassignedMarkers = Markers.guardMarkers.filter(m => !m.assignee);

        if (unassignedMarkers.length == 0) return result;

        for (const guard of unassignedGuards)
        {
            const nearestMarker = unassignedMarkers.at(0); // Positions.closestByRange(guard, unassignedMarkers);

            if (!nearestMarker) console.log("no nearestMarker");
            if (!nearestMarker) continue;

            guard.sentry = nearestMarker.pos;
            nearestMarker.assignee = guard.creep;
            result.add(guard);
            break;
        }

        return result;
    }
}