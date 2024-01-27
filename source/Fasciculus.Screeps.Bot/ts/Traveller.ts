import { Vector } from "./Common";
import { Directions } from "./Geometry";
import { Positions } from "./Positions";

export class Path
{
    readonly goal: RoomPosition;
    readonly directions: Vector<DirectionConstant>;

    constructor(goal: RoomPosition, directions: Vector<DirectionConstant>)
    {
        this.goal = goal;
        this.directions = directions;
    }

    encode(): string
    {
        return `${Positions.encode(this.goal)}:${Directions.encode(this.directions)}`;
    }

    static decode(value?: string): Path | undefined
    {
        if (!value) return undefined;

        let parts: string[] = value.split(":");

        if (parts.length != 2) return undefined;

        let goal: RoomPosition | undefined = Positions.decode(parts[0]);

        if (!goal) return undefined;

        let directions: Vector<DirectionConstant> = Directions.decode(parts[1]);

        return new Path(goal, directions);
    }
}

export class Traveller
{
    static moveTo(creep: Creep, target: RoomPosition | { pos: RoomPosition }, range: number = 1)
    {
        if (creep.fatigue > 0) return;

        let goal: RoomPosition = target instanceof RoomPosition ? target : target.pos;

        // creep.move()
    }
}