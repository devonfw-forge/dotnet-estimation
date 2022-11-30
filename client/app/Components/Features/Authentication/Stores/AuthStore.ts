import create from "zustand";
import { persist } from "zustand/middleware";

interface IAuthState {
  user: String | undefined;
  login: (username: String) => void;
}

export const useAuthStore = create<IAuthState>()(
  persist((set, get) => ({
    user: undefined,
    login: (username: String) => set((state) => ({ ...state, user: username })),
  }))
);
