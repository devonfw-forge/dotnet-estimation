import { FunctionComponent } from "react";
import {
  convertTypeToColorIfActive,
  EstimationType,
} from "../../../../../Types/EstimationType";
import { useTaskStore } from "../../Tasks/Stores/TaskStore";
import { useEstimationStore } from "../Stores/EstimationStore";

export const EstimationValue: FunctionComponent<{
  value: number;
  gridColumn: number;
  isActive: boolean;
  parentType: EstimationType;
  isFinal: boolean;
}> = ({ value, gridColumn, isActive, parentType, isFinal }) => {
  const { setValue } = useEstimationStore();
  const { setFinalComplexity } = useTaskStore();

  return (
    <>
      <button
        className={"btn hover:border-blue-800 border-2"}
        style={{
          gridColumn: gridColumn,
          gridRow: 1,
          width: "36px",
          height: "36px",
          borderRadius: "50%",
          background: convertTypeToColorIfActive(parentType, isActive),
        }}
        onClick={() => {
          if (isFinal) {
            console.log("Setting final value: " + value);
            setFinalComplexity(value);
          }
          setValue(parentType, value);
        }}
      >
        {value}
      </button>
    </>
  );
};
