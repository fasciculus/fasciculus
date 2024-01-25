import { Vector } from "./Collections";

export type Positioned = RoomPosition | _HasRoomPosition;

export class Positions
{
    static positionOf(target: Positioned): RoomPosition
    {
        return target instanceof RoomPosition ? target : target.pos;
    }

    static closestByPath<T extends Positioned>(start: Positioned, targets: Vector<T>, opts?: FindPathOpts): T | undefined
    {
        return targets.find((values) => Positions.positionOf(start).findClosestByPath(values, opts) || undefined);
    }
}