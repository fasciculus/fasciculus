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

    static encode(pos: RoomPosition): string
    {
        return `${pos.roomName}.${pos.x}.${pos.y}`;
    }

    static decode(encoded: string): RoomPosition | undefined
    {
        let parts: string[] = encoded.split(".");

        if (parts.length != 3) return undefined;

        let x: number = Number(parts[1]);
        let y: number = Number(parts[2]);

        if (isNaN(x) || isNaN(y)) return undefined;

        return new RoomPosition(x, y, parts[0]);
    }
}