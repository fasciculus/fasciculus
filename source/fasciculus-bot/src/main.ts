
import { ES } from "./es/es";
import { Screeps } from "./screeps/screeps";

ES.setup();

export const loop = function ()
{
    const emojis: Array<string> = ["🚛", "🏹", "🔰", "⚕️", "⛏️", "👷", "⚔️", "⚜️", "😈"];
    const infos: Array<string> = emojis.map(e => `${e}(${e.length})`);

    Screeps.setup();
    console.log(infos);
    Screeps.cleanup();
}
