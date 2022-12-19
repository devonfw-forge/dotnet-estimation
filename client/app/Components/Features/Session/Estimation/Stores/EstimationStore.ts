import produce from "immer";
import create from "zustand";
import { EstimationType } from "../../../../../Types/EstimationType";

interface IEstimationState {
  complexity: number;
  effort: number;
  risk: number;
  resetStore: () => void;
  setValue: (type: EstimationType, value: number) => void;
}

export const useEstimationStore = create<IEstimationState>()((set) => ({
  complexity: 1,
  effort: 1,
  risk: 1,
  resetStore: () =>
    set((state) => ({ ...state, complexity: 1, effort: 1, risk: 1 })),
  setValue: (type: EstimationType, value: number) => {
    set(
      produce((draft) => {
        draft[type] = value;
      })
    );
  },
}));

interface IFinalComplexityState {
  finalValue: number;
  setValue: (value: number) => void;
  resetStore: () => void;
}

//noch mal angucken wg. effort und risk einfach auf 0 gestetzt und bie set rausgenommen
export const useFinalValueStore = create<IFinalComplexityState>()((set) => ({
  finalValue: 1,
  resetStore: () =>
    set((state) => ({ ...state, finalValue: 1})),
  setValue: (value: number) => {
    set(
      produce((draft) => {
        draft = value;
      })
    );
  },
}));

