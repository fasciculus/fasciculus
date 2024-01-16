
import { INameMemory } from "./INameMemory";
import { ISpringMemory } from "./ISpringMemory";

export interface IMemory extends Memory
{
    names?: INameMemory;

    springs?: { [name: string]: ISpringMemory };
}