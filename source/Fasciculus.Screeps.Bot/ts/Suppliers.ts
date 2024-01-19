import * as _ from "lodash";

import { Bodies } from "./Bodies";
import { CreepBase, Creeps } from "./Creeps";
import { CreepState, CreepType } from "./Enums";
import { Extensions } from "./Extensions";
import { GameWrap } from "./GameWrap";
import { SupplierMemory } from "./Memories";
import { Spawns } from "./Spawns";
import { Customer, IdCustomer, IdSupply, Supply } from "./Types";
import { Utils } from "./Utils";

const MIN_SUPPLY_ENERGY = 10;

const SUPPLIER_MOVE_TO_OPTS: MoveToOpts =
{
    visualizePathStyle:
    {
        stroke: "#ff0"
    }
};

export class Supplier extends CreepBase
{
    get memory(): SupplierMemory { return super.memory as SupplierMemory; }

    get supply(): Supply | undefined { return GameWrap.get(this.memory.supply); }
    set supply(value: Supply | undefined) { this.memory.supply = value?.id; }

    get customer(): Customer | undefined { return GameWrap.get(this.memory.customer); }
    set customer(value: Customer | undefined) { this.memory.customer = value?.id; }

    constructor(creep: Creep)
    {
        super(creep);
    }

    execute()
    {
        switch (this.state)
        {
            case CreepState.ToSupply: this.executeToSupply(); break;
            case CreepState.ToCustomer: this.executeToCustomer(); break;
            case CreepState.Withdraw: this.executeWithdraw(); break;
            case CreepState.Transfer: this.executeTransfer(); break;
        }
    }

    private executeToSupply()
    {
        let supply = this.supply;

        if (!supply) return;

        this.moveTo(supply, SUPPLIER_MOVE_TO_OPTS);
    }

    private executeToCustomer()
    {
        let customer = this.customer;

        if (!customer) return;

        this.moveTo(customer, SUPPLIER_MOVE_TO_OPTS);
    }

    private executeWithdraw()
    {
        let supply = this.supply;

        if (!supply) return;

        this.withdraw(supply, RESOURCE_ENERGY);
    }

    private executeTransfer()
    {
        let customer = this.customer;

        if (!customer) return;

        this.transfer(customer, RESOURCE_ENERGY);

        if (customer instanceof Creep)
        {
            this.customer = undefined;
            this.state = CreepState.Idle;
        }
    }

    prepare()
    {
        switch (this.state)
        {
            case CreepState.Idle: this.state = this.prepareIdle();
            case CreepState.ToSupply: this.state = this.prepareMoveToSupply();
            case CreepState.ToCustomer: this.state =  this.prepareMoveToCustomer();
            case CreepState.Withdraw: this.state =  this.prepareWithdraw();
            case CreepState.Transfer: this.state = this.prepareTransfer();
        }
    }

    private prepareIdle(): CreepState
    {
        if (this.energy == 0)
        {
            let supply = this.supply;

            this.customer = undefined;

            if (!supply) return CreepState.Idle;

            return this.inRangeTo(supply) ? CreepState.Withdraw : CreepState.ToSupply;
        }
        else
        {
            let customer = this.customer;

            this.supply = undefined;

            if (!customer) return CreepState.Idle;

            return this.inRangeTo(customer) ? CreepState.Transfer : CreepState.ToCustomer;
        }
    }

    private prepareMoveToSupply(): CreepState
    {
        let supply = this.supply;

        if (!supply) return this.prepareIdle();

        if (!Supplier.hasEnergy(supply))
        {
            this.supply = undefined;
            return this.prepareIdle();
        }

        return this.inRangeTo(supply) ? CreepState.Withdraw : CreepState.ToSupply;
    }

    private prepareMoveToCustomer(): CreepState
    {
        let customer = this.customer;

        if (!customer) return this.prepareIdle();

        if (!Supplier.hasCapacity(customer))
        {
            this.customer = undefined;
            return this.prepareIdle();
        }

        return this.inRangeTo(customer) ? CreepState.Transfer : CreepState.ToCustomer;
    }

    private prepareWithdraw(): CreepState
    {
        let supply = this.supply;

        if (!supply) return this.prepareIdle();

        if (this.freeEnergyCapacity == 0 || !Supplier.hasEnergy(supply))
        {
            this.supply = undefined;
            return this.prepareIdle();
        }

        return this.inRangeTo(supply) ? CreepState.Withdraw : CreepState.ToSupply;
    }

    private prepareTransfer(): CreepState
    {
        let customer = this.customer;

        if (!customer) return this.prepareIdle();

        if (this.energy == 0 || !Supplier.hasCapacity(customer))
        {
            this.customer = undefined;
            return this.prepareIdle();
        }

        return this.inRangeTo(customer) ? CreepState.Transfer : CreepState.ToCustomer;
    }

    private inRangeTo(target: Supply | Customer): boolean
    {
        return this.pos.inRangeTo(target, 1);
    }

    static hasEnergy(supply: Supply): boolean
    {
        return supply.store.energy >= MIN_SUPPLY_ENERGY;
    }

    static hasCapacity(customer: Customer): boolean
    {
        let minCapacity = customer instanceof Creep ? 20 : 0;

        return customer.store.getFreeCapacity(RESOURCE_ENERGY) > minCapacity;
    }
}

const SUPPLIER_PARTS: BodyPartConstant[] = [CARRY, MOVE, CARRY, MOVE, CARRY, MOVE, CARRY, MOVE, CARRY, MOVE];
const SUPPLIER_MIN_SIZE = 2;

export class Suppliers
{
    private static _all: Supplier[] = [];

    static get all(): Supplier[] { return Suppliers._all; }

    static initialize()
    {
        Suppliers._all = Creeps.ofType(CreepType.Supplier).map(c => new Supplier(c));

        Bodies.register(CreepType.Supplier, SUPPLIER_MIN_SIZE, SUPPLIER_PARTS);
    }

    static run()
    {
        Suppliers._all.forEach(s => s.prepare());
        Suppliers.assign();
        Suppliers._all.forEach(s => s.prepare());
        Suppliers._all.forEach(s => s.execute());
    }

    private static assign()
    {
        Suppliers.assignSupplies();
        Suppliers.assignCustomers();
    }

    private static assignSupplies()
    {
        let unassignedSuppliers: _.Dictionary<Supplier> = _.indexBy(Suppliers._all.filter(s => s.energy == 0 && !s.supply), s => s.name);

        if (_.size(unassignedSuppliers) == 0) return;

        let assignedSupplies: Set<IdSupply> = new Set(Utils.defined(Suppliers._all.map(s => s.memory.supply)));

        let wellers: Supply[] = Creeps.ofType(CreepType.Weller).filter(Supplier.hasEnergy);
        let unassignedSupplies: Supply[] = wellers.filter(c => !assignedSupplies.has(c.id));
        let sortedSupplies: Supply[] = unassignedSupplies.sort(Suppliers.compareSupplies);

        for (let supply of sortedSupplies)
        {
            let assignables: Supplier[] = _.values(unassignedSuppliers);
            let supplier = supply.pos.findClosestByPath(assignables) || undefined;

            if (!supplier) continue;

            supplier.supply = supply;
            delete unassignedSuppliers[supplier.name];

            if (_.size(unassignedSuppliers) == 0) break;
        }
    }

    private static assignCustomers()
    {
        let unassignedSuppliers: _.Dictionary<Supplier> = _.indexBy(Suppliers._all.filter(s => s.energy > 0 && !s.customer), s => s.name);

        if (_.size(unassignedSuppliers) == 0) return;

        let assignedCustomers: Set<IdCustomer> = new Set(Utils.defined(Suppliers._all.map(s => s.customer?.id)));

        let spawns: Customer[] = Spawns.my.filter(Supplier.hasCapacity);
        let extensions: Customer[] = Extensions.my.filter(Supplier.hasCapacity);
        let upgraders: Customer[] = Creeps.ofType(CreepType.Upgrader).filter(Supplier.hasCapacity);
        let builders: Customer[] = Creeps.ofType(CreepType.Builder).filter(Supplier.hasCapacity);

        let unassignedCustomers = spawns.concat(extensions).concat(upgraders).concat(builders).filter(c => !assignedCustomers.has(c.id));
        let sortedCustomers = unassignedCustomers.sort(Suppliers.compareCustomers);

        for (let customer of sortedCustomers)
        {
            let assignables: Supplier[] = _.values(unassignedSuppliers);
            let supplier = customer.pos.findClosestByPath(assignables) || undefined;

            if (!supplier) continue;

            supplier.customer = customer;
            delete unassignedSuppliers[supplier.name];

            if (_.size(unassignedSuppliers) == 0) break;
        }
    }

    private static compareSupplies(a: Supply, b: Supply): number
    {
        return b.store.energy - a.store.energy;
    }

    private static compareCustomers(a: Customer, b: Customer): number
    {
        return b.store.getFreeCapacity(RESOURCE_ENERGY) - a.store.getFreeCapacity(RESOURCE_ENERGY)
    }
}