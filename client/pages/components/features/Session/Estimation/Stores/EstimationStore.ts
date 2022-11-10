import produce from "immer";
import create from "zustand";
import { Type } from "../../../../../Types/Type";

interface IEstimationState {
  complexity: number;
  effort: number;
  risk: number;
  resetStore: () => void;
  setValue: (type: Type, value: number) => void;
}

export const useEstimationStore = create<IEstimationState>()((set) => ({
  complexity: 1,
  effort: 1,
  risk: 1,
  resetStore: () =>
    set((state) => ({ ...state, complexity: 1, effort: 1, risk: 1 })),
  setValue: (type: Type, value: number) => {
    set(
      produce((draft) => {
        draft[type] = value;
      })
    );
  },
}));
