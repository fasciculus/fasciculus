
import { ES } from "./es/es";
import { Screeps } from "./screeps/screeps";

ES.setup();

export const loop = function ()
{
    Screeps.setup();

    console.log(`idle spawns ${StructureSpawn.idle}`);

    Screeps.cleanup();
}
