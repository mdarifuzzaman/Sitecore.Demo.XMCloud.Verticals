
import { ComponentParams, ComponentRendering, Placeholder } from '@sitecore-jss/sitecore-jss-nextjs';
import React from 'react'

interface ComponentProps {
    rendering: ComponentRendering & { params: ComponentParams };
    params: ComponentParams;
  }

export default function ContainerComponent(props: ComponentProps) {
  console.log("ContainerComponent", props);
  const phKey = `nested`;
  const id = props.params?.RenderingIdentifier;
  return (
    <div className={`component container-default`} id={id ? id : undefined}>
      <div className="component-content">
        <div className="row">
          <Placeholder name={phKey} rendering={props.rendering} />
        </div>
      </div>
    </div>
  )
}
