
import { profile } from "./Profiling";
import { Chamber, Chambers } from "./Rooms";

const PATH_MAX_COST: number = 1000000;

export type Positioned = RoomPosition | _HasRoomPosition;

export class PositionHelper
{
    static positionOf(target: Positioned): RoomPosition
    {
        return target instanceof RoomPosition ? target : target.pos;
    }
}

class PathPart
{
    readonly key: string;
    readonly direction: DirectionConstant;
    readonly cost: number;

    accessed: number;

    constructor(key: string, direction: DirectionConstant, cost: number)
    {
        this.key = key;
        this.direction = direction;
        this.cost = cost;

        this.accessed = Game.time;
    }
}

export class Paths
{
    private static paths: Map<string, PathPart> = new Map();
    private static blocks: Map<string, RoomPosition> = new Map();

    private static posKey(p: RoomPosition): string
    {
        return p.roomName + "_" + p.x + "_" + p.y;
    }

    private static pathKey(s: RoomPosition, g: RoomPosition, range: number): string
    {
        return s.roomName + "_" + s.x + "_" + s.y + "_" + g.roomName + "_" + g.x + "_" + g.y + "_" + range;
    }

    private static direction(from: RoomPosition, to: RoomPosition): DirectionConstant
    {
        const dx = to.x - from.x;
        const dy = to.y - from.y;

        if (dx < 0)
        {
            return dy < 0 ? TOP_LEFT : (dy > 0 ? BOTTOM_LEFT : LEFT);
        }
        else if (dx > 0)
        {
            return dy < 0 ? TOP_RIGHT : (dy > 0 ? BOTTOM_RIGHT : RIGHT);
        }
        else
        {
            return dy < 0 ? TOP : BOTTOM;
        }
    }

    private static callback(roomName: string | undefined): boolean | CostMatrix
    {
        const chamber: Chamber | undefined = Chambers.get(roomName);
        var costMatrix: CostMatrix = chamber ? chamber.costMatrix : new PathFinder.CostMatrix();
        const blocks: Map<string, RoomPosition> = Paths.blocks;

        if (blocks.size > 0)
        {
            costMatrix = costMatrix.clone();

            for (const block of blocks.values())
            {
                if (block.roomName == roomName)
                {
                    costMatrix.set(block.x, block.y, 255);
                }
            }
        }

        return costMatrix;
    }

    private static findWithCreeps(start: RoomPosition, goal: RoomPosition, range: number): PathPart | undefined
    {
        const chamber: Chamber | undefined = Chambers.get(start.roomName);

        if (!chamber) return undefined;

        if (start.roomName != goal.roomName)
        {
            const direction = chamber.room.findExitTo(goal.roomName);

            if (direction != FIND_EXIT_TOP && direction != FIND_EXIT_RIGHT && direction !== FIND_EXIT_BOTTOM && direction != FIND_EXIT_LEFT) return undefined;

            const exit = start.findClosestByRange(direction);

            if (!exit) return undefined;

            goal = exit;
        }

        const opts: FindPathOpts = { range };
        const steps: PathStep[] = chamber.room.findPath(start, goal, opts);

        if (!steps || steps.length == 0) return undefined;

        return new PathPart("", steps[0].direction, 1);
    }

    @profile
    static find(start: RoomPosition, goal: RoomPosition, range: number, ignoreCreeps: boolean): PathPart | undefined
    {
        if (!ignoreCreeps)
        {
            return Paths.findWithCreeps(start, goal, range);
        }

        const pathKey = Paths.pathKey(start, goal, range);
        const paths = Paths.paths;
        var result: PathPart | undefined = paths.get(pathKey);

        if (result && Paths.blocks.has(result.key))
        {
            paths.delete(pathKey);
            result = undefined;
        }

        if (result)
        {
            result.accessed = Game.time;
        }
        else
        {
            const opts: PathFinderOpts = { roomCallback: Paths.callback };
            const pfp: PathFinderPath = PathFinder.search(start, { pos: goal, range }, opts);
            const length = pfp.path.length;

            if (!pfp.incomplete && length > 0)
            {
                const path = pfp.path;
                const cost: number = pfp.cost;

                var lastPos = start;

                for (var i = 0, n = path.length; i < n; ++i)
                {
                    const subKey = Paths.pathKey(lastPos, goal, range);
                    const subPos = path[i];
                    const posKey = Paths.posKey(subPos);
                    const direction = Paths.direction(lastPos, subPos);
                    const subCost = cost * (length - i) / length;

                    paths.set(subKey, new PathPart(posKey, direction, subCost))

                    lastPos = subPos;
                }

                result = paths.get(pathKey);
            }
        }

        return result;
    }

    static cost(start: RoomPosition, goal: RoomPosition, range: number): number
    {
        return Paths.find(start, goal, range, true)?.cost || PATH_MAX_COST;
    }

    @profile
    static closest<T extends Positioned>(start: Positioned, goals: Array<T>, range: number): T | undefined
    {
        const startPos = PositionHelper.positionOf(start);

        return goals.min(goal => Paths.cost(startPos, PositionHelper.positionOf(goal), range));
    }

    static block(creep: Creep)
    {
        Paths.blocks.set(creep.name, creep.pos);
    }

    @profile
    static cleanup()
    {
        const paths = Paths.paths;
        const blocks = Paths.blocks;
        const threshold = Game.time - 1500;

        paths.keep(paths.find(pp => pp.accessed > threshold));
        blocks.keep(Game.myCreepNames);

        console.log(`Path cache has ${Paths.paths.size} entries and ${Paths.blocks.size} blocks.`);
    }
}

export class Mover
{
    @profile
    static moveTo(creep: Creep, target: Positioned, range: number, ignoreCreeps: boolean = true): CreepMoveReturnCode | ERR_NO_PATH
    {
        if (creep.fatigue > 0) return ERR_TIRED;

        const start: RoomPosition = creep.pos;
        const goal: RoomPosition = PositionHelper.positionOf(target)
        const path: PathPart | undefined = Paths.find(start, goal, range, ignoreCreeps);

        if (!path) console.log(`no path for ${creep.name}`);
        if (!path) return ERR_NO_PATH;

        const direction: DirectionConstant = path.direction;

        return creep.move(direction);
    }
}