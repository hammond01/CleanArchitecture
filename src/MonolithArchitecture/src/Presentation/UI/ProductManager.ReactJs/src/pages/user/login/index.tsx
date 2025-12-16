import React from 'react';
import { PageContainer } from '@ant-design/pro-components';
import { Button, Result } from 'antd';
import { Link } from '@umijs/max';

// Temporary: Login disabled
export default function LoginDisabled(): React.ReactElement {
  return (
    <PageContainer>
      <Result
        status="warning"
        title="Login is temporarily disabled"
        subTitle="The login feature has been temporarily disabled by the administrators. Please try again later or contact the system administrator."
        extra={[
          <Button type="primary" key="home">
            <Link to="/">Go to Home</Link>
          </Button>,
        ]}
      />
    </PageContainer>
  );
}


