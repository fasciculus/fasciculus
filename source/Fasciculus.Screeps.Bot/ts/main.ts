
import { Vector } from "./Common";
import { Profiler } from "./Profiling";
import { Chambers } from "./Rooming";
import { Scheduler } from "./Scheduler";

function roomCallback(name: string): boolean | CostMatrix
{
    return new PathFinder.CostMatrix();
}

function pos2str(p: RoomPosition): string
{
    return p.roomName + "_" + p.x + "_" + p.y;
}

export const loop = function ()
{
    Profiler.start();

    Scheduler.initialize();
    Scheduler.run();

    //const spawn: StructureSpawn = Game.spawns["Spawn1"];
    //const flag: Flag = Game.flags["U1"];
    //const opts: PathFinderOpts = { roomCallback };
    //const pfp: PathFinderPath = PathFinder.search(spawn.pos, flag.pos, opts);
    //const path: Vector<RoomPosition> = Vector.from(pfp.path);

    //console.log(`incomplete: ${pfp.incomplete}, cost: ${pfp.cost}, ops: ${pfp.ops}, length: ${path.length}`);
    //console.log(`  ${path.map(pos2str).toArray()}`);

    const room: Room = Chambers.get("W7N3")?.room!;

    Profiler.stop();
}