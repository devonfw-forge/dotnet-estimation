import produce from "immer";
import create from "zustand";
import { IUser } from "../../../../../../app/Interfaces/IUser";

interface ISessionUserState {
  users: IUser[];
  setCurrentUsers(payload: IUser[]): void;
  addUser(payload: IUser): void;
  removeUser(id: String): void;
}

export const useSessionUserStore = create<ISessionUserState>()((set) => ({
  users: [],
  addUser: (payload: IUser) =>
    set(
      produce((draft) => {
        draft.users.push(payload);
      })
    ),
  setCurrentUsers: (payload: IUser[]) =>
    set((state) => ({ ...state, users: payload })),
  removeUser: (id: String) =>
    set(
      produce((draft) => {
        draft.users.filter((item: IUser) => item.id === id);
      })
    ),
}));
