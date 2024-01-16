import { Bots } from "./Bots";
import { Walls } from "./Walls";

export class Initializer
{
    static run()
    {
        Walls.initialize();
        Bots.initialize();
    }
}