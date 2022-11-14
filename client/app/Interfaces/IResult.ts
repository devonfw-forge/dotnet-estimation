export interface IVoteResult {
    value: Number;
}

export interface ITaskResult {
    uuid: String,
    title: String,
    results: IVoteResult[],
}

export interface IResult {
    results: ITaskResult[],
}
