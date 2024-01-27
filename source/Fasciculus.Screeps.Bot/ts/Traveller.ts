import { Vector } from "./Common";

export class Path
{
    readonly goal: RoomPosition;
    readonly directions: Vector<DirectionConstant>;

    constructor(goal: RoomPosition, directions: Vector<DirectionConstant>)
    {
        this.goal = goal;
        this.directions = directions;
    }
}

export class Traveller
{
}