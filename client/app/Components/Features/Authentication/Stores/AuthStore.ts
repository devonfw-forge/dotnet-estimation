import create from "zustand";
import { Role } from "../../../../Types/Role";

interface IAuthState {
  username: String | undefined;
  token: String | undefined;
  userId: String | undefined;
  role: Role | undefined;
  login: (username: String, token: String, userId: String, role: Role) => void;
  isAdmin: () => boolean;
}

export const useAuthStore = create<IAuthState>()((set, get) => ({
  username: undefined,
  token: undefined,
  userId: undefined,
  role: undefined,
  login: (username: String, token: String, userId: String, role: Role) =>
    set((state) => ({ ...state, username, token, userId, role })),
  isAdmin: () => {
    return get().role === Role.Admin;
  },
  isLoggedIn: () => {
    return get().userId && get().username && get().token && get().role;
  },
}));
