
export type ContainerId = Id<StructureContainer>;
export type ControllerId = Id<StructureController>;
export type SiteId = Id<ConstructionSite>;
export type SourceId = Id<Source>;

export type IdSupply = Id<Creep | StructureContainer>;
export type Supply = Creep | StructureContainer;

export type IdCustomer = Id<Creep | StructureSpawn | StructureExtension>;
export type Customer = Creep | StructureSpawn | StructureExtension;

export type IdRepairable = Id<StructureRoad | StructureWall>
export type Repairable = StructureRoad | StructureWall
