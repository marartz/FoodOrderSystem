export class ImportLogLineModel {

  constructor(init?: Partial<ImportLogLineModel>) {
    if (init) {
      Object.assign(this, init);
    }
  }

  public timestamp: string;

  public type: number;

  public rowIndex: number;

  public message: string;

  public clone(): ImportLogLineModel {
    return new ImportLogLineModel({
      timestamp: this.timestamp,
      type: this.type,
      rowIndex: this.rowIndex,
      message: this.message
    });
  }

}
