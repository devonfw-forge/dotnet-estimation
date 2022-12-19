import produce from "immer";
import create from "zustand";
import { IUser } from "../../../../../../app/Interfaces/IUser";
import { IUpdateDto } from "../../../../../Interfaces/IUpdateDto";

interface ISessionUserState {
  users: IUser[];
  setCurrentUsers(payload: IUser[]): void;
  addUser(payload: IUser): void;
  removeUser(id: String): void;
  changeOnlineStatus(payload: IUpdateDto): void;
}

export const useSessionUserStore = create<ISessionUserState>()((set) => ({
  users: [],
  addUser: (payload: IUser) =>
    set(
      produce((draft) => {
        draft.users.push(payload);
        draft.users.forEach((user: IUser) => {
          if(user.id.trim() === payload.id)
          {
            user.online = true;
            user.role = payload.role;
          }
        });
        draft.users.filter((user: IUser) => user.token === null)
        draft.users = uniqBy(draft.users, JSON.stringify);
        console.log("useradd:"+ JSON.stringify(draft.users));
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
  changeOnlineStatus: (payload: IUpdateDto) => set(
    produce((draft) => {
      if(payload.availableClients != undefined && payload.availableClients.length != 0)
      {
        console.log("currently online:"+ JSON.stringify(payload.availableClients));
        draft.users.forEach((user: IUser) => {
          payload.availableClients.forEach(onlineUser => {
            if(user.id.trim() === onlineUser.trim())
            {
              user.online = true;
            }
          });
        });
        draft.users.filter((user: IUser) => {
          if(!payload.availableClients.includes(user.id))
          {
            user.online = false;
          }
        });
        draft.users = draft.users.filter((user: IUser) => user.online === true);
        console.log("Online Users:"+ JSON.stringify(draft.users));
      }
    })
  ),
}));

function uniqBy(user: IUser[], key: any) {
  let seen = new Set();
  return user.filter(item => {
      let k = key(item);
      return seen.has(k) ? false : seen.add(k);
  });
}