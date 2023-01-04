import React from "react";
import { FunctionComponent } from "react";
interface CheckboxPoperties {
  size: string;
  backgroundColor: string;
  accentColor: string;
}
export const Checkbox: FunctionComponent<CheckboxPoperties> = ({
  size,
  backgroundColor,
  accentColor,
}) => {
  const style = { width: size, height: size };
  return (
    <svg
      className="checkmark"
      xmlns="http://www.w3.org/2000/svg"
      style={style}
      viewBox="0 0 52 52"
    >
      <circle
        className="checkmark__circle"
        cx="26"
        cy="26"
        r="25"
        fill={backgroundColor}
      />
      <path
        className="checkmark__check"
        d="M14.1 27.2l7.1 7.2 16.7-16.8"
        stroke={accentColor}
        fill="none"
        strokeWidth="6"
      />
    </svg>
  );
};