import { Type } from "../Types/Type";

export interface ITypedMessage {
  type: Type;
}

export interface IMessage<T> extends ITypedMessage {
  payload: T;
}
