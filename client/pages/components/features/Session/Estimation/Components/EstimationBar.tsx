import { FunctionComponent, useState } from "react";
import { EstimationValue } from "./EstimationButton";
import { useEstimationStore } from "../../Estimation/Stores/EstimationStore";
import { Type } from "../../../../../Types/Type";

export const EstimationBar: FunctionComponent<{ type: Type }> = ({ type }) => {
  const state = useEstimationStore();

  const validValues = [1, 2, 3, 5, 8, 13, 21];

  return (
    <>
      <div className="flex flex-col">
        <div
          style={{
            display: "grid",
            gridTemplateColumns: 14,
            alignItems: "center",
          }}
        >
          {validValues.map((item) => {
            return (
              <EstimationValue
                value={item}
                key={"estimationValue" + item}
                gridColumn={validValues.indexOf(item) * 2 + 2}
                isActive={item == state[type] ? true : false}
                parentType={type}
              />
            );
          })}
        </div>
      </div>
    </>
  );
};
