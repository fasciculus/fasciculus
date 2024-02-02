import { Dictionaries, Vector } from "./Common";
import { profile } from "./Profiling";
import { Chamber, Chambers } from "./Rooms";

const PATH_MAX_COST: number = 1000000;
const PATHS_MAX_ENTRIES: number = 1000;

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

    @profile
    static find(start: RoomPosition, goal: RoomPosition, range: number): PathPart | undefined
    {
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
            console.log(`finding new path ${pathKey}`);
            const opts: PathFinderOpts = { roomCallback: Paths.callback };
            const pfp: PathFinderPath = PathFinder.search(start, { pos: goal, range }, opts);
            const length = pfp.path.length;

            if (!pfp.incomplete && length > 0)
            {
                console.log(`found new path ${pathKey}`);
                const path = pfp.path;
                const cost: number = pfp.cost;

                var lastPos = start;

                for (var i = 0, n = path.length; i < n; ++i)
                {
                    const subKey = Paths.pathKey(lastPos, goal, range);
                    const subPos = path[i];
                    const posKey = Paths.posKey(subPos);
                    const direction = lastPos.getDirectionTo(subPos);
                    const subCost = cost * (length - i) / length;

                    paths.set(subKey, new PathPart(posKey, direction, subCost))

                    lastPos = subPos;
                }

                result = paths.get(pathKey);
            }

            if (!result)
            {
                console.log(`no new path ${pathKey}`);
            }
        }

        return result;
    }

    static cost(start: RoomPosition, goal: RoomPosition, range: number): number
    {
        return Paths.find(start, goal, range)?.cost || PATH_MAX_COST;
    }

    @profile
    static closest<T extends Positioned>(start: Positioned, goals: Vector<T>, range: number): T | undefined
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
        Paths.cleanupPaths();
        Paths.cleanupBlocks();

        console.log(`Path cache has ${Paths.paths.size} entries and ${Paths.blocks.size} blocks.`);
    }

    private static cleanupPaths()
    {
        const paths = Paths.paths;

        if (paths.size <= PATHS_MAX_ENTRIES) return;

        const toDelete: Array<string> = new Array();
        const threshold = Game.time - 1500;

        paths.forEach((v, k) => { if (v.accessed < threshold) toDelete.push(k) });
        toDelete.forEach(k => paths.delete(k));
    }

    private static cleanupBlocks()
    {
        const blocks = Paths.blocks;
        const existing: Set<string> = Dictionaries.keys(Game.creeps);
        const toDelete: Array<string> = new Array();

        for (const key of blocks.keys())
        {
            if (!existing.has(key))
            {
                toDelete.push(key);
            }
        }

        toDelete.forEach(k => blocks.delete(k));
    }

    private static older(a: [string, PathPart], b: [string, PathPart]): number
    {
        return a[1].accessed - b[1].accessed;
    }
}

export class Mover
{
    @profile
    static moveTo(creep: Creep, goal: Positioned, range: number): CreepMoveReturnCode | ERR_NO_PATH
    {
        const start: RoomPosition = creep.pos;
        const path: PathPart | undefined = Paths.find(start, PositionHelper.positionOf(goal), range);

        if (!path) console.log(`no path for ${creep.name}`);
        if (!path) return ERR_NO_PATH;

        const direction: DirectionConstant = path.direction;

        return creep.move(direction);
    }
}