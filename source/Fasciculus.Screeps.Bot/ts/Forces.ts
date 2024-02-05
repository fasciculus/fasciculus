import { CreepState, CreepType, Dictionaries, Dictionary, Positions, SerializedPosition, Vector } from "./Common";
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
        switch (this.state)
        {
            case CreepState.Idle: this.state = this.prepareIdle(); break;
            case CreepState.ToPosition: this.state = this.prepareToPosition(); break;
        }
    }

    private prepareIdle(): CreepState
    {
        if (this.guarding) return CreepState.Idle;

        const sentry = this.sentry;

        if (!sentry) return CreepState.Idle;

        if (this.pos.isEqualTo(sentry))
        {
            this.guarding = true;
            return CreepState.Idle;
        }

        return CreepState.ToPosition;
    }

    private prepareToPosition(): CreepState
    {
        const sentry = this.sentry;

        if (!sentry) return CreepState.Idle;

        if (this.pos.isEqualTo(sentry))
        {
            this.guarding = true;
            return CreepState.Idle;
        }

        return CreepState.ToPosition;
    }

    execute()
    {
        switch (this.state)
        {
            case CreepState.ToPosition: this.executeToPosition(); break;
        }
    }

    private executeToPosition()
    {
        const sentry = this.sentry;

        if (!sentry) return;

        this.moveTo(sentry, 0);
    }
}

export class Guards
{
    private static _guards: Map<string, Guard> = new Map();

    private static _all: Array<Guard> = new Array();

    static get count(): number { return Guards._all.length; }

    @profile
    static initialize()
    {
        if (Creeps.update(Guards._guards, CreepType.Guard, name => new Guard(name)))
        {
            Guards._all = Guards._guards.vs();
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
    private static prepare(guards: Array<Guard>)
    {
        guards.forEach(b => b.prepare());
    }

    @profile
    private static execute(guards: Array<Guard>)
    {
        guards.forEach(b => b.execute());
    }

    @profile
    private static assign(): Array<Guard>
    {
        const result: Array<Guard> = new Array();

        if (Markers.guardMarkerCount == 0) return result;

        const unassignedGuards = Guards._all.filter(g => !g.sentry);

        if (unassignedGuards.length == 0) return result;

        const unassignedMarkers = Markers.guardMarkers.filter(m => !m.assignee);

        if (unassignedMarkers.length == 0) return result;

        for (const guard of unassignedGuards)
        {
            const nearestMarker = unassignedMarkers[0]; // Positions.closestByRange(guard, unassignedMarkers);

            if (!nearestMarker) console.log("no nearestMarker");
            if (!nearestMarker) continue;

            guard.sentry = nearestMarker.pos;
            nearestMarker.assignee = guard.creep;
            result.push(guard);
            break;
        }

        return result;
    }
}