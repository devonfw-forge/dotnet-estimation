import { FunctionComponent } from "react";
import { convertTypeToColorIfActive, Type } from "../../../../../Types/Type";
import { useEstimationStore } from "../../Estimation/Stores/EstimationStore";

export const EstimationValue: FunctionComponent<{
  value: number;
  gridColumn: number;
  isActive: boolean;
  parentType: Type;
}> = ({ value, gridColumn, isActive, parentType }) => {
  const { setValue } = useEstimationStore();

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
          setValue(parentType, value);
        }}
      >
        {value}
      </button>
    </>
  );
};
