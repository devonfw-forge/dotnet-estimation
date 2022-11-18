import { useRouter } from "next/router";
import { useState } from "react";
import JoinSession from "../app/Components/Features/Join/JoinSession";
import CreateSession from "../app/Components/Features/Join/CreateSession";
import { Frame } from "../app/Components/Globals/Frame";

export default function Home() {
  enum FormSelector {
    Join,
    Create,
  }

  const [selector, setSelector] = useState<FormSelector>(0);

  const defaultPadding = "p-4";

  const setFormToRender = (element: any) => {
    setSelector(element.target.value);
  };

  return (
    <>
      <div>
        <input
          type="radio"
          value={FormSelector.Join}
          name="join"
          checked={selector == FormSelector.Join}
          onChange={setFormToRender}
        />
        Join
        <input
          type="radio"
          value={FormSelector.Create}
          name="create"
          checked={selector == FormSelector.Create}
          onChange={setFormToRender}
        />
        Create
      </div>
      <div>{selector == 0 ? <JoinSession /> : <CreateSession />}</div>
    </>
  );
}
